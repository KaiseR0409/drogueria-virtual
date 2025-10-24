using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DrogueriaAPI.Data;
using DrogueriaAPI.Models;
using DrogueriaAPI.Models.DTOs;

namespace DrogueriaAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DireccionesController : ControllerBase
    {
        private readonly DrogueriaDbContext _context;

        public DireccionesController(DrogueriaDbContext context)
        {
            _context = context;
        }

        // Obtener todas las direcciones de un usuario
        [HttpGet("usuario/{idUsuario}")]
        public async Task<ActionResult<IEnumerable<DireccionDTO>>> GetDireccionesByUsuario(int idUsuario)
        {
            var direcciones = await _context.Direcciones
                .Where(d => d.IdUsuario == idUsuario)
                .ToListAsync();

            if (direcciones == null || direcciones.Count == 0)
                return NotFound(new { mensaje = "El usuario no tiene direcciones registradas." });

            return direcciones.Select(d => new DireccionDTO
            {
                IdDireccion = d.IdDireccion,
                IdUsuario = d.IdUsuario,
                Etiqueta = d.Etiqueta,
                Region = d.Region,
                Comuna = d.Comuna,
                Calle = d.Calle,
                NumeroCalle = d.NumeroCalle,
                Complemento = d.Complemento,
                EsPrincipal = d.EsPrincipal
            }).ToList();
        }

        //Crear nueva dirección para un usuario
        [HttpPost("usuario/{idUsuario}")]
        public async Task<IActionResult> AddDireccion(int idUsuario, [FromBody] DireccionDTO dto)
        {
            var usuario = await _context.Usuarios.FindAsync(idUsuario);
            if (usuario == null)
                return NotFound(new { mensaje = "Usuario no encontrado." });

            var direccion = new Direccion
            {
                IdUsuario = idUsuario,
                Etiqueta = dto.Etiqueta,
                Region = dto.Region,
                Comuna = dto.Comuna,
                Calle = dto.Calle,
                NumeroCalle = dto.NumeroCalle,
                Complemento = dto.Complemento,
                EsPrincipal = dto.EsPrincipal
            };

            // Si esta dirección es principal, desmarca otras
            if (dto.EsPrincipal)
            {
                var otras = await _context.Direcciones
                    .Where(d => d.IdUsuario == idUsuario && d.EsPrincipal)
                    .ToListAsync();

                foreach (var d in otras)
                    d.EsPrincipal = false;
            }

            _context.Direcciones.Add(direccion);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetDireccionesByUsuario), new { idUsuario = direccion.IdUsuario }, dto);
        }

        // Actualizar una dirección existente
        [HttpPut("{idDireccion}")]
        public async Task<IActionResult> UpdateDireccion(int idDireccion, [FromBody] DireccionDTO dto)
        {
            var direccion = await _context.Direcciones.FindAsync(idDireccion);
            if (direccion == null)
                return NotFound(new { mensaje = "Dirección no encontrada." });

            direccion.Etiqueta = dto.Etiqueta;
            direccion.Region = dto.Region;
            direccion.Comuna = dto.Comuna;
            direccion.Calle = dto.Calle;
            direccion.NumeroCalle = dto.NumeroCalle;
            direccion.Complemento = dto.Complemento;

            // Si la marcamos como principal, desmarca otras
            if (dto.EsPrincipal && !direccion.EsPrincipal)
            {
                var otras = await _context.Direcciones
                    .Where(d => d.IdUsuario == direccion.IdUsuario && d.EsPrincipal)
                    .ToListAsync();

                foreach (var d in otras)
                    d.EsPrincipal = false;

                direccion.EsPrincipal = true;
            }

            await _context.SaveChangesAsync();
            return Ok(new { mensaje = "Dirección actualizada correctamente." });
        }

        // Eliminar una dirección
        [HttpDelete("{idDireccion}")]
        public async Task<IActionResult> DeleteDireccion(int idDireccion)
        {
            var direccion = await _context.Direcciones.FindAsync(idDireccion);
            if (direccion == null)
                return NotFound(new { mensaje = "Dirección no encontrada." });

            _context.Direcciones.Remove(direccion);
            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Dirección eliminada correctamente." });
        }

        //Marcar una dirección como principal
        [HttpPut("{idDireccion}/principal")]
        public async Task<IActionResult> SetPrincipal(int idDireccion)
        {
            var direccion = await _context.Direcciones.FindAsync(idDireccion);
            if (direccion == null)
                return NotFound(new { mensaje = "Dirección no encontrada." });

            var otras = await _context.Direcciones
                .Where(d => d.IdUsuario == direccion.IdUsuario && d.EsPrincipal)
                .ToListAsync();

            foreach (var d in otras)
                d.EsPrincipal = false;

            direccion.EsPrincipal = true;
            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Dirección marcada como principal." });
        }
    }
}
