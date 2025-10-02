using DrogueriaAPI.Models;
using System.ComponentModel.DataAnnotations;

public class ItemOrden
{
    [Key]
    public int IdItemOrden { get; set; }

    public int IdOrden { get; set; }
    public int IdProducto { get; set; }

    public int Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }

    // Escalar a factura (detalles)
    public decimal Subtotal { get; set; } // Cantidad * PrecioUnitario
    public decimal Impuesto { get; set; } // IVA individual
    public decimal Descuento { get; set; }

    // Relación con la cabecera (orden)
    public Orden Orden { get; set; }
    public Producto Producto { get; set; }
}