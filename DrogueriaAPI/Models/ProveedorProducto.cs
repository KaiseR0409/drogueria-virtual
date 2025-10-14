using DrogueriaAPI.Models;
using System.ComponentModel.DataAnnotations;

public class ProveedorProducto
{
    // Claves Compuestas: IdProveedor y IdProducto

    public int IdProveedor { get; set; }
    public int IdProducto { get; set; }

    // Datos de la Relación
    public decimal Precio { get; set; }
    public int Stock { get; set; }

    // Propiedades de Navegación
    public Proveedor? Proveedor { get; set; }
    public Producto? Producto { get; set; }
}