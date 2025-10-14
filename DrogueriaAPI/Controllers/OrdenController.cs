using DrogueriaAPI.Models.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DrogueriaAPI.Data;
using DrogueriaAPI.Models;

namespace DrogueriaAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")] // <- tu endpoint será api/orden

    public class OrdenController : ControllerBase
    {
        private readonly DrogueriaDbContext _context;
        //definir el endpoint
        public OrdenController(DrogueriaDbContext context)
        {
            _context = context;
        }


        [HttpPost] //api/orden
        public async Task<IActionResult> CrearOrden([FromBody] CrearOrdenRequest request)
        {
            if (request.Items == null || !request.Items.Any())
                return BadRequest("La orden debe tener al menos un producto.");

            var orden = new Orden
            {
                IdUsuario = request.IdUsuario,
                IdProveedor = request.IdProveedor,
                FechaOrden = DateTime.Now,
                EstadoOrden = "Pendiente",
                MontoTotal = request.MontoTotal,

                // Factura
                NumeroOrdenCompra = "OC-" + DateTime.Now.Ticks, //TEMPORAL: El cliente me deberá decir que es lo que quiere, si autogenerarlo o la api de pago lo trae
                NumeroFactura = request.NumeroFactura,
                TipoComprobante = request.TipoComprobante,
                FechaFactura = request.FechaFactura,
                MetodoPago = request.MetodoPago,
                Moneda = request.Moneda,
                Impuestos = request.Impuestos,
                Descuento = request.Descuento,
                DireccionEnvioCompleta = request.DireccionEnvioCompleta,
            };

            _context.Ordenes.Add(orden);
            await _context.SaveChangesAsync();

            foreach (var item in request.Items)
            {
                var itemOrden = new ItemOrden
                {
                    IdOrden = orden.IdOrden,
                    IdProducto = item.IdProducto,
                    Cantidad = item.Cantidad,
                    PrecioUnitario = item.PrecioUnitario,
                    Impuesto = item.Impuesto,
                    Descuento = item.Descuento
                };
                _context.ItemsOrden.Add(itemOrden);
            }

            await _context.SaveChangesAsync();

            return Ok(new
            {
                orden.IdOrden,
                orden.MontoTotal,
                orden.EstadoOrden,
                orden.NumeroFactura,
                orden.FechaFactura,
                Items = request.Items
            });
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrden(int id)
        {
            var orden = await _context.Ordenes
                .Include(o => o.Items)
                .ThenInclude(i => i.Producto)
                .Include(o => o.Proveedor)
                .FirstOrDefaultAsync(o => o.IdOrden == id);

            if (orden == null)
            {
                return NotFound();
            }

            return Ok(new
            {
                IdOrden = orden.IdOrden,
                FechaOrden = orden.FechaOrden,
                MontoTotal = orden.MontoTotal,

                Proveedor = new
                {
                    NombreProveedor = orden.Proveedor.NombreProveedor,
                },

                Items = orden.Items.Select(i => new {
                    NombreProducto = i.Producto.NombreProducto,
                    Cantidad = i.Cantidad,
                    PrecioUnitario = i.PrecioUnitario,
                })
            });
        }

        // Obtener todas las órdenes
        [HttpGet]
        public async Task<IActionResult> GetOrdenes()
        {
            var ordenes = await _context.Ordenes
                .Include(o => o.Items)
                .ThenInclude(i => i.Producto)
                .ToListAsync();

            return Ok(ordenes.Select(o => new
            {
                o.IdOrden,
                o.IdUsuario,
                o.IdProveedor,
                o.FechaOrden,
                o.EstadoOrden,
                o.MontoTotal,
                o.NumeroFactura,
                o.FechaFactura,
                Items = o.Items.Select(i => new
                {
                    i.IdProducto,
                    Producto = i.Producto.NombreProducto,
                    i.Cantidad,
                    i.PrecioUnitario
                })
            }));
        }
        // Confirmar pago y generar factura
        [HttpPut("{id}/confirmar-pago")]
        public async Task<IActionResult> ConfirmarPago(int id, [FromBody] ConfirmarPagoRequest request)
        {
            var orden = await _context.Ordenes
                .Include(o => o.Items)
                .ThenInclude(i => i.Producto)
                .FirstOrDefaultAsync(o => o.IdOrden == id);

            if (orden == null)
                return NotFound(new { mensaje = "Orden no encontrada" });


            if (orden.EstadoOrden == "Pagada")
                return BadRequest(new { mensaje = "La orden ya está pagada" });

            // Actualizar datos de la orden como factura
            orden.NumeroFactura = request.NumeroFactura;
            orden.NumeroOrdenCompra = "OC-" + DateTime.Now.Ticks;  //TEMPORAL: El cliente me deberá decir que es lo que quiere, si autogenerarlo o la api de pago lo trae
            orden.TipoComprobante = request.TipoComprobante;
            orden.MetodoPago = request.MetodoPago;
            orden.Moneda = request.Moneda;
            orden.Impuestos = request.Impuestos;
            orden.Descuento = request.Descuento;
            orden.FechaFactura = DateTime.Now;
            orden.EstadoOrden = "Pagada";

            //descontar stock
            foreach (var item in orden.Items)
            {
                var inventario = await _context.ProveedorProducto
            .FirstOrDefaultAsync(pp =>
                pp.IdProducto == item.IdProducto &&
                pp.IdProveedor == orden.IdProveedor // Usa el proveedor de la orden
            );

                // Verifica si el inventario existe y si hay suficiente stock
                if (inventario == null)
                {
                    return StatusCode(500, new { mensaje = $"Error: no se encontró el inventario" });
                }
                // Descontar el stock
                if (inventario.Stock < item.Cantidad)
                {
                    return BadRequest(new { mensaje = $"No hay suficiente stock para el producto ID {item.IdProducto}" });
                }
                inventario.Stock -= item.Cantidad;

            }
            await _context.SaveChangesAsync();

            return Ok(new
            {
                orden.IdOrden,
                orden.NumeroFactura,
                orden.TipoComprobante,
                orden.NumeroOrdenCompra,
                orden.MetodoPago,
                orden.Moneda,
                orden.FechaFactura,
                orden.MontoTotal,
                orden.Impuestos,
                orden.Descuento,
                orden.EstadoOrden,
                Items = orden.Items.Select(i => new
                {
                    i.IdProducto,
                    i.Cantidad,
                    i.PrecioUnitario
                })
            });
        }
        //  Obtener facturas por proveedor
        [HttpGet("mis-facturas/{idProveedor}")]
        public IActionResult MisFacturas(int idProveedor)
        {
            try
            {
                var facturas = _context.Ordenes
                    .Where(o => o.IdProveedor == idProveedor)
                    .OrderByDescending(o => o.FechaFactura)
                    .Select(o => new
                    {
                        o.IdOrden,
                        o.IdUsuario,
                        o.IdProveedor,
                        o.FechaOrden,
                        o.EstadoOrden,
                        o.MontoTotal,
                        o.NumeroFactura,
                        o.FechaFactura,
                        Items = o.Items.Select(i => new
                        {
                            i.IdProducto,
                            i.Producto,
                            i.Cantidad,
                            i.PrecioUnitario
                        })
                    })
                    .ToList();

                return Ok(facturas);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error interno", error = ex.Message });
            }
        }
        // GET: api/Orden/mi-historial/{idUsuario}
        [HttpGet("mi-historial/{idUsuario}")]
        public async Task<IActionResult> GetHistorialPorUsuario(int idUsuario)
        {
            try
            {
                
                var historial = await _context.Ordenes
                    .Where(o => o.IdUsuario == idUsuario)
                    .Include(o => o.Items)
                    .ThenInclude(i => i.Producto)
                    .Include(o => o.Proveedor)
                    .OrderByDescending(o => o.FechaOrden)
                    .ToListAsync();

                

                if (!historial.Any())
                {
                    return NotFound(new { mensaje = "No se encontraron órdenes para este usuario." });
                }

                
                // Devolvemos solo los datos esenciales para la vista
                return Ok(historial.Select(o => new
                {
                    o.IdOrden,
                    o.IdUsuario,
                    o.IdProveedor,
                    o.FechaOrden,
                    o.EstadoOrden,
                    o.MontoTotal,
                    o.NumeroFactura,
                    o.FechaFactura,
                    NombreProveedor = o.Proveedor.NombreProveedor,
                    Items = o.Items.Select(i => new
                    {
                        i.IdProducto,
                        NombreProducto = i.Producto.NombreProducto, 
                        i.Cantidad,
                        i.PrecioUnitario
                    }),
                    
                    // Proveedor = o.Proveedor.NombreProveedor // REQUIERE .Include(o => o.Proveedor)
                }));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error interno al obtener el historial de compras.", error = ex.Message });
            }



        }
    }
}
