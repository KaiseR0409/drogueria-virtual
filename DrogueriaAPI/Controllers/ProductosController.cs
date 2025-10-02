using DrogueriaAPI.Data;
using DrogueriaAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace DrogueriaAPI.Controllers
{

    [Route("api/[controller]")] //ruta base para el controlador: api/Productos
    [ApiController]
    public class ProductosController : ControllerBase
    {
        private readonly DrogueriaDbContext _context;

        public ProductosController(DrogueriaDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Producto>>> GetProductos(
        string? nombreProducto,
        string? principioActivo,
        string? laboratorioFabricante,
        string? formaFarmaceutica,
        [FromQuery(Name = "laboratoriosSeleccionados")] List<string>? laboratoriosSeleccionados) // Nuevo parámetro
        {
            var query = _context.Productos.AsQueryable();

            // Filtros de texto (AND implícito)
            if(!string.IsNullOrEmpty(nombreProducto))
                query = query.Where(p => p.NombreProducto.Contains(nombreProducto));
            if(!string.IsNullOrEmpty(principioActivo))
                query = query.Where(p => p.PrincipioActivo.Contains(principioActivo));
            if(!string.IsNullOrEmpty(laboratorioFabricante))
                query = query.Where(p => p.LaboratorioFabricante.Contains(laboratorioFabricante));
            if(!string.IsNullOrEmpty(formaFarmaceutica))
                query = query.Where(p => p.FormaFarmaceutica.Contains(formaFarmaceutica));
                // Filtro por checkboxes de laboratorios (OR implícito)
                // El frontend enviará una lista de laboratorios seleccionados
                if (laboratoriosSeleccionados != null && laboratoriosSeleccionados.Any())
                {
                    query = query.Where(p => laboratoriosSeleccionados.Contains(p.LaboratorioFabricante));
                }

                return await query.ToListAsync();
        }
        // GET: api/Productos por id
        [HttpGet("{id}")]
        public async Task<ActionResult<Producto>> GetProducto(int id)
        {
            var producto = await _context.Productos.FindAsync(id);

            if (producto == null)
            {
                return NotFound();
            }

            return producto;
        }
        //GET LABORATORIOS
        [HttpGet("Laboratorios")]
        public async Task<ActionResult<IEnumerable<string>>> GetLaboratorios()
        {
            var laboratorios = await _context.Productos
                                             .Select(p => p.LaboratorioFabricante)
                                             .Distinct()
                                             .ToListAsync();

            return laboratorios;
        }
        // POST: api/Productos
        [HttpPost]
        public async Task<ActionResult<Producto>> PostProducto(Producto producto)
        {
            _context.Productos.Add(producto);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetProductos", new { id = producto.IdProducto }, producto);
        }
        // DELETE: api/Productos/
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProducto(int id)
        {
            var producto = await _context.Productos.FindAsync(id);
            if (producto == null)
            {
                return NotFound();
            }
            _context.Productos.Remove(producto);
            await _context.SaveChangesAsync();
            return NoContent();
        }
        // PUT: api/Productos/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProducto(int id, Producto producto)
        {
            if (id != producto.IdProducto)
            {
                return BadRequest();
            }

            _context.Entry(producto).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductoExists(id))
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
        private bool ProductoExists(int id)
        {
            return _context.Productos.Any(e => e.IdProducto == id);
        }
    }

}
