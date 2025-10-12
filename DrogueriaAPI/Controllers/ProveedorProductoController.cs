using DrogueriaAPI.Data;
using DrogueriaAPI.Models;
using DrogueriaAPI.Models.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Route("api/proveedor")]
[ApiController]
public class ProveedorProductoController : ControllerBase
{
    private readonly DrogueriaDbContext _context;

    public ProveedorProductoController(DrogueriaDbContext context)
    {
        _context = context;
    }

    // POST: api/proveedor/{idProveedor}/producto (Publicar Producto - Doble Inserción)
    [HttpPost("{idProveedor}/producto")]
    public async Task<IActionResult> PublicarProducto(int idProveedor, [FromBody] ProductoPublicacionDto dto)
    {
        var proveedor = await _context.Proveedor.FindAsync(idProveedor);
        if (proveedor == null)
        {
            return NotFound($"Proveedor con ID {idProveedor} no encontrado.");
        }

        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            // INSERCIÓN 1: Crear el Producto
            var nuevoProducto = new Producto
            {
                NombreProducto = dto.NombreProducto,
                PrincipioActivo = dto.PrincipioActivo,
                Concentracion = dto.Concentracion,
                FormaFarmaceutica = dto.FormaFarmaceutica,
                PresentacionComercial = dto.PresentacionComercial,
                LaboratorioFabricante = dto.LaboratorioFabricante,
                RegistroSanitario = dto.RegistroSanitario,
                FechaVencimiento = dto.FechaVencimiento,
                CondicionesAlmacenamiento = dto.CondicionesAlmacenamiento,
                ImagenUrl = dto.ImagenUrl, // Usar la URL devuelta por el FilesController
                Marca = dto.Marca,
                CodigoBarras = dto.CodigoBarras
            };

            _context.Productos.Add(nuevoProducto);
            await _context.SaveChangesAsync();

            // INSERCIÓN 2: Crear el Inventario (ProveedorProducto)
            var inventario = new ProveedorProducto
            {
                IdProveedor = idProveedor,
                IdProducto = nuevoProducto.IdProducto,
                Precio = dto.Precio,
                Stock = dto.Stock
            };

            _context.ProveedorProducto.Add(inventario);
            await _context.SaveChangesAsync();

            await transaction.CommitAsync();

            return CreatedAtAction(nameof(GetInventarioItem),
                                   new { idProveedor = idProveedor, idProducto = nuevoProducto.IdProducto },
                                   inventario);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            return StatusCode(500, $"Error al publicar el producto: {ex.Message}");
        }
    }
    // POST: api/proveedor/{idProveedor}/carga-masiva
    [HttpPost("{idProveedor}/carga-masiva")]
    public async Task<IActionResult> CargaMasivaDesdeJson(int idProveedor, [FromBody] List<ProductoPublicacionDto> productosNuevos)
    {
        if (productosNuevos == null || !productosNuevos.Any())
        {
            return BadRequest(new { mensaje = "La lista de productos no puede estar vacía." });
        }

        var proveedor = await _context.Proveedor.FindAsync(idProveedor);
        if (proveedor == null)
        {
            return NotFound($"Proveedor con ID {idProveedor} no encontrado.");
        }


        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            foreach (var dto in productosNuevos)
            {
                if (string.IsNullOrEmpty(dto.NombreProducto) || dto.Precio <= 0)
                {
                    await transaction.RollbackAsync();
                    return BadRequest(new { mensaje = $"El producto '{dto.NombreProducto ?? "desconocido"}' tiene datos inválidos." });
                }

                var nuevoProducto = new Producto
                {
                    NombreProducto = dto.NombreProducto,
                    PrincipioActivo = dto.PrincipioActivo,
                    Concentracion = dto.Concentracion,
                    FormaFarmaceutica = dto.FormaFarmaceutica,
                    PresentacionComercial = dto.PresentacionComercial,
                    LaboratorioFabricante = dto.LaboratorioFabricante,
                    RegistroSanitario = dto.RegistroSanitario,
                    FechaVencimiento = dto.FechaVencimiento,
                    CondicionesAlmacenamiento = dto.CondicionesAlmacenamiento,
                    ImagenUrl = dto.ImagenUrl,
                    Marca = dto.Marca,
                    CodigoBarras = dto.CodigoBarras
                };
                _context.Productos.Add(nuevoProducto);

                var inventario = new ProveedorProducto
                {
                    Proveedor = proveedor,
                    Producto = nuevoProducto,
                    Precio = dto.Precio,
                    Stock = dto.Stock
                };
                _context.ProveedorProducto.Add(inventario);
            }

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return Ok(new { mensaje = $"{productosNuevos.Count} productos han sido publicados exitosamente." });
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            return StatusCode(500, new { mensaje = "Ocurrió un error al guardar los productos.", error = ex.Message });
        }
    }

    // GET: api/proveedor/{idProveedor}/productos (Listado para el Dashboard)
    [HttpGet("{idProveedor}/productos")]
    public async Task<ActionResult<IEnumerable<ProveedorProducto>>> GetProductosDelProveedor(int idProveedor)
    {
        var productos = await _context.ProveedorProducto
            .Where(pp => pp.IdProveedor == idProveedor)
            .Include(pp => pp.Producto)
            .ToListAsync();

        if (!productos.Any())
        {
            return NotFound("Este proveedor no tiene productos publicados.");
        }
        return Ok(productos);
    }
    // PUT: api/proveedor/{idProveedor}/producto/{idProducto} (Editar Producto e Inventario)
    [HttpPut("{idProveedor}/producto/{idProducto}")]
    public async Task<IActionResult> EditarProductoCompleto(int idProveedor, int idProducto, [FromBody] ProductoPublicacionDto dto)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            //ACTUALIZAR PRODUCTO (Tabla Productos)
            var productoExistente = await _context.Productos.FindAsync(idProducto);

            if (productoExistente == null)
            {
                return NotFound($"Producto con ID {idProducto} no encontrado.");
            }

            // Mapear el DTO a la entidad Producto (Actualización de campos farmacéuticos)
            productoExistente.NombreProducto = dto.NombreProducto;
            productoExistente.PrincipioActivo = dto.PrincipioActivo;
            productoExistente.Concentracion = dto.Concentracion;
            productoExistente.FormaFarmaceutica = dto.FormaFarmaceutica;
            productoExistente.PresentacionComercial = dto.PresentacionComercial;
            productoExistente.LaboratorioFabricante = dto.LaboratorioFabricante;
            productoExistente.RegistroSanitario = dto.RegistroSanitario;
            productoExistente.FechaVencimiento = dto.FechaVencimiento;
            productoExistente.CondicionesAlmacenamiento = dto.CondicionesAlmacenamiento;
            productoExistente.ImagenUrl = dto.ImagenUrl;
            productoExistente.Marca = dto.Marca;
            productoExistente.CodigoBarras = dto.CodigoBarras;

            _context.Productos.Update(productoExistente);
            await _context.SaveChangesAsync();

            // 2. ACTUALIZAR INVENTARIO (Tabla ProveedorProducto)
            var inventarioItem = await _context.ProveedorProducto
                .FirstOrDefaultAsync(pp => pp.IdProveedor == idProveedor && pp.IdProducto == idProducto);

            if (inventarioItem == null)
            {
                
                return NotFound($"Inventario no encontrado para Proveedor {idProveedor} y Producto {idProducto}.");
            }

            // Actualizar el Precio y Stock
            inventarioItem.Precio = dto.Precio;
            inventarioItem.Stock = dto.Stock;

            _context.ProveedorProducto.Update(inventarioItem);
            await _context.SaveChangesAsync();

            await transaction.CommitAsync();

            // Devolvemos 204 No Content para una actualización exitosa
            return NoContent();
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            return StatusCode(500, $"Error al editar el producto: {ex.Message}");
        }
    }

   
    // PUT: api/proveedor/{idProveedor}/inventario/{idProducto} (Editar Precio y Stock)
    [HttpPut("{idProveedor}/inventario/{idProducto}")]
    public async Task<IActionResult> UpdateInventario(int idProveedor, int idProducto, [FromBody] InventarioUpdateDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var item = await _context.ProveedorProducto
            .FirstOrDefaultAsync(pp => pp.IdProveedor == idProveedor && pp.IdProducto == idProducto);

        if (item == null)
        {
            return NotFound("Inventario no encontrado para este proveedor/producto.");
        }

        if (dto.Precio.HasValue)
        {
            item.Precio = dto.Precio.Value;
        }
        if (dto.Stock.HasValue)
        {
            item.Stock = dto.Stock.Value;
        }

        try
        {
            await _context.SaveChangesAsync();
            return NoContent();
        }
        catch (DbUpdateConcurrencyException)
        {
            return StatusCode(500, "Error de concurrencia al actualizar.");
        }
    }

    // DELETE: api/proveedor/{idProveedor}/inventario/{idProducto} (Eliminar Inventario del Proveedor)
    [HttpDelete("{idProveedor}/inventario/{idProducto}")]
    public async Task<IActionResult> EliminarInventario(int idProveedor, int idProducto)
    {
        var item = await _context.ProveedorProducto
            .FirstOrDefaultAsync(pp => pp.IdProveedor == idProveedor && pp.IdProducto == idProducto);

        if (item == null)
        {
            return NotFound("El producto ya no está en el inventario de este proveedor.");
        }

        _context.ProveedorProducto.Remove(item);

        try
        {
            await _context.SaveChangesAsync();
            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error al eliminar: {ex.Message}");
        }
    }

    // GET: api/proveedor/{idProveedor}/producto/{idProducto} (Método auxiliar)
    [HttpGet("{idProveedor}/producto/{idProducto}")]
    public async Task<ActionResult<ProveedorProducto>> GetInventarioItem(int idProveedor, int idProducto)
    {
        var item = await _context.ProveedorProducto
            .FirstOrDefaultAsync(pp => pp.IdProveedor == idProveedor && pp.IdProducto == idProducto);

        if (item == null)
        {
            return NotFound();
        }
        return item;
    }
    // GET: api/ProveedorProducto/InventarioCompletoConFiltros
    // Este endpoint está diseñado para el MODO COMPRADOR, trayendo todos los productos con sus opciones de proveedor
    [HttpGet("/api/ProveedorProducto/InventarioCompletoConFiltros")]
    public async Task<ActionResult<IEnumerable<ProveedorProducto>>> GetInventarioCompletoConFiltros(
        [FromQuery] string? nombreProducto,
        [FromQuery] string? principioActivo,
        [FromQuery] string? laboratorioFabricante,
        [FromQuery] string? formaFarmaceutica,
        [FromQuery] List<string>? laboratoriosSeleccionados) // Si los filtros son del menú
    {
        // Consulta Base: Traer todos los items de inventario (ProveedorProducto)
        // Incluir Producto y Proveedor para tener todos los datos anidados que el frontend necesita.
        var query = _context.ProveedorProducto
            .Include(pp => pp.Producto)
            .Include(pp => pp.Proveedor)
            .AsQueryable();

        // Aplicar Filtros (si se proporcionan)
        if (!string.IsNullOrWhiteSpace(nombreProducto))
        {
            query = query.Where(pp => pp.Producto.NombreProducto.ToLower().Contains(nombreProducto.ToLower()));
        }

        if (!string.IsNullOrWhiteSpace(principioActivo))
        {
            query = query.Where(pp => pp.Producto.PrincipioActivo.ToLower().Contains(principioActivo.ToLower()));
        }

        // Si usas el campo de Laboratorio de búsqueda libre
        if (!string.IsNullOrWhiteSpace(laboratorioFabricante))
        {
            query = query.Where(pp => pp.Producto.LaboratorioFabricante.ToLower().Contains(laboratorioFabricante.ToLower()));
        }

        if (!string.IsNullOrWhiteSpace(formaFarmaceutica))
        {
            query = query.Where(pp => pp.Producto.FormaFarmaceutica.ToLower().Contains(formaFarmaceutica.ToLower()));
        }

        
        if (laboratoriosSeleccionados != null && laboratoriosSeleccionados.Any())
        {
            query = query.Where(pp => laboratoriosSeleccionados.Contains(pp.Producto.LaboratorioFabricante));
        }

        // 3. Ejecutar la Consulta
        var inventarioCompleto = await query.ToListAsync();

        // 4. Manejo de Respuesta
        if (!inventarioCompleto.Any())
        {
            // Devolvemos 200 OK con una lista vacía si no hay coincidencias
            return Ok(new List<ProveedorProducto>());
        }

        return Ok(inventarioCompleto);
    }
    //GET: api/ProveedorProducto/BuscarSimilares
    [HttpGet("BuscarSimilares")]
    public async Task<ActionResult<IEnumerable<object>>> BuscarSimilares(
        [FromQuery] string? nombreProducto,
        [FromQuery] string? principioActivo,
        [FromQuery] int? idProductoExcluir = null)
    {
        var query = _context.ProveedorProducto
            .Include(pp => pp.Producto)
            .Include(pp => pp.Proveedor)
            .Where(pp => pp.Stock > 0)
            .AsQueryable();

        //lógica de similitud por Nombre o Principio Activo
        if (!string.IsNullOrWhiteSpace(nombreProducto))
        {
            query = query.Where(pp => pp.Producto.NombreProducto.ToLower().Contains(nombreProducto.ToLower()));
        }
        else if (!string.IsNullOrWhiteSpace(principioActivo))
        {
            query = query.Where(pp => pp.Producto.PrincipioActivo.ToLower().Contains(principioActivo.ToLower()));
        }
        else
        {
            return Ok(new List<object>());
        }

        // excluir el ID del producto que abrió el modal
        if (idProductoExcluir.HasValue)
        {
            query = query.Where(pp => pp.IdProducto != idProductoExcluir.Value);
        }

        // ejecutar la Consulta
        var productosSimilares = await query
            .OrderByDescending(pp =>
                !string.IsNullOrWhiteSpace(nombreProducto)
                ? pp.Producto.NombreProducto.ToLower().StartsWith(nombreProducto.ToLower())
                : !string.IsNullOrWhiteSpace(principioActivo)
                    ? pp.Producto.PrincipioActivo.ToLower().StartsWith(principioActivo.ToLower())
                    : false 
            )
            .ThenBy(pp => pp.Precio) 
            .Take(50)
            .ToListAsync();

        // mapear la respuesta al formato simple que necesita Svelte
        var resultado = productosSimilares.Select(pp => new
        {
            IdProducto = pp.IdProducto,
            IdProveedor = pp.IdProveedor,
            NombreProducto = pp.Producto.NombreProducto,
            Precio = pp.Precio,
            Stock = pp.Stock,
            NombreProveedor = pp.Proveedor.NombreProveedor,
        }).ToList();

        return Ok(resultado);
    }
}