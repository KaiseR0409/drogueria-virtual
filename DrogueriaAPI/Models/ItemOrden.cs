using DrogueriaAPI.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

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
    [JsonIgnore]
    [ForeignKey(nameof(IdOrden))]
    public Orden Orden { get; set; }

    [ForeignKey(nameof(IdProducto))]
    public Producto Producto { get; set; }
}