using DrogueriaAPI.Data;
using DrogueriaAPI.Models;
using DrogueriaAPI.Models.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

// Nota: No necesitas los usings de JWT/Security para este controlador específico.

namespace DrogueriaAPI.Controllers
{
    // Nombre del controlador corregido y simplificado:
    [Route("api/[controller]")]
    [ApiController]
    public class ProveedorController : ControllerBase
    {
        private readonly DrogueriaDbContext _context;

        public ProveedorController(DrogueriaDbContext context)
        {
            _context = context;
        }

        // POST: api/Proveedor/registrar/{idUsuario}
        [HttpPost("registrar/{idUsuario}")]
        public async Task<IActionResult> RegistrarProveedor(int idUsuario, [FromBody] ProveedorCreationDto proveedorDto)
        {
            
            var usuario = await _context.Usuarios.FindAsync(idUsuario);
            if (usuario == null)
            {
                return NotFound($"El usuario con ID {idUsuario} no existe.");
            }

            
            var proveedorExistente = await _context.Proveedores.FindAsync(idUsuario);
            if (proveedorExistente != null)
            {
                return Conflict($"El usuario con ID {idUsuario} ya está registrado como proveedor.");
            }

            // Crear el objeto Proveedor (Usando la clave compartida)
            var proveedor = new Proveedor
            {
                
                IdProveedor = idUsuario,

                // Lógica de nombre
                NombreProveedor = string.IsNullOrEmpty(proveedorDto.NombreProveedor)
                                        ? usuario.NombreUsuario
                                        : proveedorDto.NombreProveedor
            };

            
            _context.Proveedores.Add(proveedor);

            try
            {
                await _context.SaveChangesAsync(); 

               
                usuario.TipoUsuario = "Proveedor";
                _context.Entry(usuario).State = EntityState.Modified;
                await _context.SaveChangesAsync(); 

                // Devuelve 201 Created
                return CreatedAtAction(nameof(GetProveedor), new { id = proveedor.IdProveedor }, proveedor);
            }
            catch (DbUpdateException ex)
            {
                // Esencial para ver si la clave foránea o el mapeo fallan.
                return StatusCode(500, $"Error al guardar el proveedor. Causa: {ex.InnerException?.Message}");
            }
        }

        // GET: api/Proveedor/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Proveedor>> GetProveedor(int id)
        {
            var proveedor = await _context.Proveedores.FindAsync(id);

            if (proveedor == null)
            {
                return NotFound();
            }

            return proveedor;
        }
        //GET: api/Proveedor para obtener el id del proveedor a través del nombre que es unico 
        [HttpGet("nombre/{nombreProveedor}")]
        public async Task<ActionResult<Proveedor>> GetProveedorByName(string nombreProveedor)
        {
            var proveedor = await _context.Proveedores
                                          .FirstOrDefaultAsync(p => p.NombreProveedor == nombreProveedor);
            if (proveedor == null)
            {
                return NotFound();
            }
            return proveedor;
        }
        // GET: api/Proveedor/Listado
        [HttpGet("Listado")]
        public async Task<IActionResult> GetProveedores()
        {
            var proveedores = await _context.Proveedores
                .Select(p => p.NombreProveedor)
                .Distinct()
                .ToListAsync();

            return Ok(proveedores);
        }
    }
}