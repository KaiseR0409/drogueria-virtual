using BCrypt.Net;
using DrogueriaAPI.Data;
using DrogueriaAPI.Models;
using DrogueriaAPI.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace DrogueriaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioController : ControllerBase
    {
        private readonly DrogueriaDbContext _context;

        public UsuarioController(DrogueriaDbContext context)
        {
            _context = context;
        }
        //crear la clase para el request de login que sirve para recibir los datos del usuario
        public class LoginRequest
        {
            public string usuario { get; set; } = string.Empty;
            public string password { get; set; } = string.Empty;
        }

        // GET: api/usuario
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Usuarios>>> GetUsuarios()
        {
            return await _context.Usuarios.ToListAsync();
        }

        // GET: api/usuario/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Usuarios>> GetUsuario(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);

            if (usuario == null)
            {
                return NotFound();
            }

            return usuario;
        }
        //endpoint para login
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            // Buscar el usuario en la base de datos
            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Usuario == request.usuario);

            if (usuario == null)
                return Unauthorized(new { mensaje = "Usuario  nulo" });

            if (string.IsNullOrWhiteSpace(request.usuario) || string.IsNullOrWhiteSpace(request.password))
                return BadRequest(new { mensaje = "Debe ingresar usuario y contraseña." });

            if (!string.Equals(usuario.EstadoUsuario, "Activo", StringComparison.OrdinalIgnoreCase))
                return Unauthorized(new { mensaje = "El usuario no está activo. Contacte al administrador." });

            // validar contraseña 
            if (!BCrypt.Net.BCrypt.Verify(request.password, usuario.Password))
                return Unauthorized(new { mensaje = "datos incorrectos" });

            // generar el JWT con claims 
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, usuario.NombreUsuario),
                new Claim("tipoUsuario", usuario.TipoUsuario.ToString()),
                new Claim("idUsuario", usuario.IdUsuario.ToString()),
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("M7f!9vB2qR#s8WxZpL6eTjQ4uKdH1mNc"));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: "DrogueriaVirtual",
                audience: "DrogueriaVirtual",
                claims: claims,
                expires: DateTime.Now.AddHours(2),
                signingCredentials: creds
            );

            // Si es proveedor, crea la respuesta con idProveedor
            if (usuario.TipoUsuario == "Proveedor")
            {
                var proveedor = await _context.Proveedores
                    .FirstOrDefaultAsync(p => p.IdUsuario == usuario.IdUsuario);

                if (proveedor != null)
                    return Ok(new
                    {
                        token = new JwtSecurityTokenHandler().WriteToken(token),
                        tipoUsuario = usuario.TipoUsuario,
                        idUsuario = usuario.IdUsuario,
                        idProveedor = proveedor.IdProveedor
                    });
            }

            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token),
                tipoUsuario = usuario.TipoUsuario,
                nombreUsuario = usuario.NombreUsuario,
                idUsuario = usuario.IdUsuario,
                idProveedor = usuario.Proveedor != null ? usuario.Proveedor.IdProveedor : (int?)null
            });
        }

        // POST: api/Usuario
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> CrearUsuario([FromBody] AgregarUsuarioRequest dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Verificar si el usuario ya existe
            var usuarioExistente = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Usuario == dto.Usuario);

            if (usuarioExistente != null)
                return Conflict("El nombre de usuario ya está en uso.");

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // Crear usuario base
                var nuevoUsuario = new Usuarios
                {
                    NombreUsuario = dto.NombreUsuario,
                    Usuario = dto.Usuario,
                    Password = dto.Password.StartsWith("$2a$")
                        ? dto.Password // si ya viene hasheada no la vuelve a hashear
                        : BCrypt.Net.BCrypt.HashPassword(dto.Password),
                    TipoUsuario = dto.TipoUsuario,
                    TipoEstablecimiento = dto.TipoEstablecimiento,
                    Direccion1 = dto.Direccion1,
                    Direccion2 = dto.Direccion2,
                    Direccion3 = dto.Direccion3,
                    EstadoUsuario = "Inactivo",
                    Correo = dto.Correo,
                    Telefono = dto.Telefono,
                    FechaCreacion = DateTime.Now,
                    FechaActualizacion = DateTime.Now,
                };

                _context.Usuarios.Add(nuevoUsuario);
                await _context.SaveChangesAsync();

                // Si es proveedor, crea la relación y sus datos
                if (dto.TipoUsuario.Equals("Proveedor", StringComparison.OrdinalIgnoreCase))
                {
                    var nuevoProveedor = new Proveedor
                    {
                        IdProveedor = nuevoUsuario.IdUsuario,
                        IdUsuario = nuevoUsuario.IdUsuario, 
                        NombreProveedor = dto.NombreProveedor ?? nuevoUsuario.NombreUsuario,
                        Giro = dto.Giro,
                        DireccionComercial = dto.DireccionComercial,
                        Ciudad = dto.Ciudad,
                        RUT = dto.Rut,
                        Usuario = nuevoUsuario
                    };

                    _context.Proveedores.Add(nuevoProveedor);
                    await _context.SaveChangesAsync();
                }

                await transaction.CommitAsync();

                return Ok(new
                {
                    mensaje = "Usuario creado exitosamente.",
                    nuevoUsuario.IdUsuario,
                    nuevoUsuario.NombreUsuario,
                    nuevoUsuario.TipoUsuario
                });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, new { mensaje = "Ocurrió un error al crear el usuario.", error = ex.Message });
            }
        }


        // PUT: api/usuario/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUsuario(int id, Usuarios usuario)
        {
            if (id != usuario.IdUsuario)
            {
                return BadRequest();
            }

            // Encontrar el usuario existente en la base de datos
            var usuarioExistente = await _context.Usuarios.AsNoTracking().FirstOrDefaultAsync(u => u.IdUsuario == id);

            if (usuarioExistente == null)
            {
                return NotFound();
            }

            // Si el campo Password viene vacio o nulo se mantiene la contraseña actual
            if (string.IsNullOrWhiteSpace(usuario.Password))
            {
                usuario.Password = usuarioExistente.Password;
            }
            else
            {
                // Solo hasheamos si no está ya hasheada
                usuario.Password = usuario.Password.StartsWith("$2a$")
                    ? usuario.Password
                    : BCrypt.Net.BCrypt.HashPassword(usuario.Password);
            }

            //Establecer la fecha de actualización
            usuario.FechaActualizacion = DateTime.Now;

            // Marcar el objeto actualizado para guardado
            _context.Entry(usuario).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UsuarioExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/usuario/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUsuario(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }

            _context.Usuarios.Remove(usuario);
            try
            {
                // Ejecutar el borrado en cascada (Usuario -> Proveedor -> ProveedorProducto)
                await _context.SaveChangesAsync();

                // Limpiar Productos Huérfanos
                await CleanUpOrphanProductsAsync(); 

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al eliminar el usuario: {ex.Message}");
            }
        }


        private async Task CleanUpOrphanProductsAsync()
        {
            // Encuentra todos los IDs de Productos que NO tienen ninguna entrada en la tabla ProveedorProducto
            var orphanProductIds = await _context.Productos
                .Where(p => !_context.ProveedorProductos.Any(pp => pp.IdProducto == p.IdProducto))
                .Select(p => p.IdProducto)
                .ToListAsync();

            if (orphanProductIds.Any())
            {
                var orphanProducts = await _context.Productos
                    .Where(p => orphanProductIds.Contains(p.IdProducto))
                    .ToListAsync();

                _context.Productos.RemoveRange(orphanProducts);


                await _context.SaveChangesAsync();
            }
        }

        private bool UsuarioExists(int id)
        {
            return _context.Usuarios.Any(e => e.IdUsuario == id);
        }

        
    }
}