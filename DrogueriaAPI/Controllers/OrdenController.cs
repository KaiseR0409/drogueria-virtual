using DrogueriaAPI.Models.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DrogueriaAPI.Data;
using DrogueriaAPI.Models;

namespace DrogueriaAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")] 

    public class OrdenController : ControllerBase
    {
        private readonly DrogueriaDbContext _context;
        //definir el endpoint
        public OrdenController(DrogueriaDbContext context)
        {
            _context = context;
        }


        [HttpPost] // POST: api/orden
        public async Task<IActionResult> CrearOrden([FromBody] CrearOrdenRequest request)
        {
            if (request == null)
                return BadRequest(new { mensaje = "Solicitud inválida." });

            if (request.IdUsuario <= 0 || request.IdProveedor <= 0)
                return BadRequest(new { mensaje = "Usuario o proveedor inválido." });

            if (request.Items == null || !request.Items.Any())
                return BadRequest(new { mensaje = "La orden debe tener al menos un producto." });

            // Verificamos que el usuario y proveedor existan
            var usuario = await _context.Usuarios.FindAsync(request.IdUsuario);
            var proveedor = await _context.Proveedores.FindAsync(request.IdProveedor);
            var direccion = await _context.Direcciones
                .FirstOrDefaultAsync(d => d.IdDireccion == request.IdDireccion && d.IdUsuario == request.IdUsuario);

            if (usuario == null)
                return NotFound(new { mensaje = "Usuario no encontrado." });

            if (proveedor == null)
                return NotFound(new { mensaje = "Proveedor no encontrado." });
            if ( direccion == null)
            {
                return NotFound(new { mensaje = "Direccion Principal no encontrada" });
            }

            // --- Calcular subtotal, IVA (19%) y total ---
            decimal subtotal = 0;
            decimal totalImpuestos = 0;
            const decimal IVA_PORCENTAJE = 0.19m;

            var items = new List<ItemOrden>();

            foreach (var i in request.Items)
            {
                var sub = i.Cantidad * i.PrecioUnitario;
                var impuesto = sub * IVA_PORCENTAJE; // 19% del subtotal de ese producto

                subtotal += sub;
                totalImpuestos += impuesto;

                items.Add(new ItemOrden
                {
                    IdProducto = i.IdProducto,
                    Cantidad = i.Cantidad,
                    PrecioUnitario = i.PrecioUnitario,
                    Impuesto = impuesto,
                    Descuento = i.Descuento
                });
            }

            decimal total = subtotal + totalImpuestos - request.Descuento;


            // --- Crear la entidad Orden ---
            var orden = new Orden
            {
                IdUsuario = request.IdUsuario,
                IdProveedor = request.IdProveedor,
                MontoTotal = total,
                NumeroFactura = request.NumeroFactura ?? "TEMP-" + DateTime.Now.Ticks,
                TipoComprobante = request.TipoComprobante ?? "Boleta",
                FechaFactura = request.FechaFactura ?? DateTime.Now,
                MetodoPago = request.MetodoPago ?? "Pendiente",
                Moneda = request.Moneda ?? "CLP",
                Impuestos = totalImpuestos,
                Descuento = request.Descuento,
                DireccionEnvioCompleta =
                    $"{direccion.Calle} {direccion.NumeroCalle}, {direccion.Comuna}, {direccion.Region}" +
                    $"{(string.IsNullOrWhiteSpace(direccion.Complemento) ? "" : ", " + direccion.Complemento)}",
                EstadoOrden = "Pendiente",
                FechaOrden = DateTime.Now
            };

            // --- Agregar los ítems ---
            orden.Items = request.Items.Select(i => new ItemOrden
            {
                IdProducto = i.IdProducto,
                Cantidad = i.Cantidad,
                PrecioUnitario = i.PrecioUnitario,
                Impuesto = i.PrecioUnitario * i.Cantidad * 0.19m,
                Descuento = i.Descuento
            }).ToList();

            // Guardar en BD
            _context.Ordenes.Add(orden);
            await _context.SaveChangesAsync();

            // --- Devolver respuesta coherente ---
            return Ok(new
            {
                orden.IdOrden,
                orden.NumeroFactura,
                orden.TipoComprobante,
                orden.FechaFactura,
                orden.MetodoPago,
                orden.Moneda,
                orden.MontoTotal,
                orden.EstadoOrden,
                orden.FechaOrden,
                Cliente = new
                {
                    usuario.IdUsuario,
                    usuario.NombreUsuario,
                    usuario.Correo,
                    DireccionEnvio = orden.DireccionEnvioCompleta
                },
                Proveedor = new
                {
                    proveedor.IdProveedor,
                    proveedor.NombreProveedor,
                    proveedor.RUT,
                    proveedor.Giro,
                    proveedor.DireccionComercial,
                    proveedor.Ciudad
                },
                Items = orden.Items.Select(i => new
                {
                    i.IdProducto,
                    i.Cantidad,
                    i.PrecioUnitario,
                    Subtotal = i.Cantidad * i.PrecioUnitario
                })
            });
        }



        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrden(int id)
        {
            var orden = await _context.Ordenes
                .Include(o => o.Items)
                    .ThenInclude(i => i.Producto)
                .Include(o => o.Proveedor)
                .Include(o => o.Usuario)
                .FirstOrDefaultAsync(o => o.IdOrden == id);

            if (orden == null)
            {
                return NotFound();
            }

            return Ok(new
            {
                orden.IdOrden,
                orden.NumeroFactura,
                orden.NumeroOrdenCompra,
                orden.TipoComprobante,
                orden.MetodoPago,
                orden.Moneda,
                orden.FechaFactura,
                orden.MontoTotal,
                orden.Impuestos,
                orden.Descuento,
                orden.EstadoOrden,
                orden.FechaOrden,

                cliente = new
                {
                    id = orden.Usuario.IdUsuario,
                    nombreUsuario = orden.Usuario.NombreUsuario,
                    correo = orden.Usuario.Correo,
                    direccionEnvioCompleta = orden.DireccionEnvioCompleta
                },

                proveedor = new
                {
                    idProveedor = orden.Proveedor.IdProveedor,
                    nombreProveedor = orden.Proveedor.NombreProveedor,
                    rut = orden.Proveedor.RUT,
                    giro = orden.Proveedor.Giro,
                    direccionComercial = orden.Proveedor.DireccionComercial,
                    ciudad = orden.Proveedor.Ciudad
                },

                items = orden.Items.Select(i => new
                {
                    i.IdProducto,
                    nombreProducto = i.Producto.NombreProducto,
                    i.Cantidad,
                    i.PrecioUnitario,
                    subtotal = i.Cantidad * i.PrecioUnitario
                })
            });
        }

        // Obtener todas las órdenes
        [HttpGet]
        [HttpGet]
        public async Task<IActionResult> GetOrdenes()
        {
            var ordenes = await _context.Ordenes
                .Include(o => o.Items)
                    .ThenInclude(i => i.Producto)
                .Include(o => o.Proveedor)
                .Include(o => o.Usuario)
                .OrderByDescending(o => o.FechaOrden)
                .ToListAsync();

            return Ok(ordenes.Select(o => new
            {
                o.IdOrden,
                o.NumeroFactura,
                o.NumeroOrdenCompra,
                o.TipoComprobante,
                o.MetodoPago,
                o.Moneda,
                o.FechaFactura,
                o.MontoTotal,
                o.Impuestos,
                o.Descuento,
                o.EstadoOrden,
                o.FechaOrden,

                cliente = new
                {
                    id = o.Usuario.IdUsuario,
                    nombreUsuario = o.Usuario.NombreUsuario,
                    correo = o.Usuario.Correo,
                    direccionEnvioCompleta = o.DireccionEnvioCompleta
                },

                proveedor = new
                {
                    idProveedor = o.Proveedor.IdProveedor,
                    nombreProveedor = o.Proveedor.NombreProveedor,
                    rut = o.Proveedor.RUT,
                    giro = o.Proveedor.Giro,
                    direccionComercial = o.Proveedor.DireccionComercial,
                    ciudad = o.Proveedor.Ciudad
                },

                items = o.Items.Select(i => new
                {
                    i.IdProducto,
                    nombreProducto = i.Producto.NombreProducto,
                    i.Cantidad,
                    i.PrecioUnitario,
                    subtotal = i.Cantidad * i.PrecioUnitario
                })
            }));
        }

        //este endpoint es para obtener el detalle de una factura (escalable para enviar por correo más adelante)
        [HttpGet("detalle/{id}")]
        public async Task<IActionResult> GetFacturaDetalle(int id)
        {
            var orden = await _context.Ordenes
                .Include(o => o.Items).ThenInclude(i => i.Producto)
                .Include(o => o.Proveedor)
                .Include(o => o.Usuario)
                .FirstOrDefaultAsync(o => o.IdOrden == id);

            if (orden == null)
                return NotFound(new { mensaje = "Orden no encontrada" });

            //Calcular subtotal, IVA y total
            decimal subtotal = orden.Items.Sum(i => i.Cantidad * i.PrecioUnitario);
            decimal iva = subtotal * 0.19m;
            decimal total = subtotal + iva - orden.Descuento;

            return Ok(new
            {
                orden.IdOrden,
                orden.NumeroFactura,
                orden.TipoComprobante,
                orden.MetodoPago,
                orden.Moneda,
                orden.FechaFactura,
                orden.FechaOrden,
                orden.EstadoOrden,
                subtotal = subtotal.ToString("N0"),
                iva = iva.ToString("N0"),
                descuento = orden.Descuento.ToString("N0"),
                total = total.ToString("N0"),

                cliente = new
                {
                    id = orden.Usuario.IdUsuario,
                    nombre = orden.Usuario.NombreUsuario,
                    correo = orden.Usuario.Correo,
                    direccionEnvioCompleta = orden.DireccionEnvioCompleta
                },

                proveedor = new
                {
                    idProveedor = orden.Proveedor.IdProveedor,
                    nombreProveedor = orden.Proveedor.NombreProveedor,
                    rut = orden.Proveedor.RUT,
                    giro = orden.Proveedor.Giro,
                    direccionComercial = orden.Proveedor.DireccionComercial,
                    ciudad = orden.Proveedor.Ciudad
                },

                items = orden.Items.Select(i => new
                {
                    i.IdProducto,
                    nombreProducto = i.Producto.NombreProducto,
                    cantidad = i.Cantidad,
                    precioUnitario = i.PrecioUnitario.ToString("N0"),
                    subtotal = (i.Cantidad * i.PrecioUnitario).ToString("N0")
                })
            });
        }



        // Confirmar pago y generar factura
        [HttpPut("{id}/confirmar-pago")]
        public async Task<IActionResult> ConfirmarPago(int id, [FromBody] ConfirmarPagoRequest request)
        {
            var orden = await _context.Ordenes
                .Include(o => o.Items)
                .ThenInclude(i => i.Producto)
                .Include(o => o.Proveedor)
                .Include(o => o.Usuario)
                .FirstOrDefaultAsync(o => o.IdOrden == id);

            if (orden == null)
                return NotFound(new { mensaje = "Orden no encontrada" });

            if (orden.EstadoOrden == "Pagada")
                return BadRequest(new { mensaje = "La orden ya está pagada" });

            orden.NumeroFactura = request.NumeroFactura;
            orden.TipoComprobante = request.TipoComprobante;
            orden.MetodoPago = request.MetodoPago;
            orden.Moneda = request.Moneda;
            orden.Impuestos = request.Impuestos;
            orden.Descuento = request.Descuento;
            orden.FechaFactura = DateTime.Now;
            orden.EstadoOrden = "Pagada";

            await _context.SaveChangesAsync();

            return Ok(new 
            { mensaje = "Pago confirmado", orden.IdOrden, orden.NumeroFactura,
                orden.EstadoOrden, orden.FechaFactura 
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
