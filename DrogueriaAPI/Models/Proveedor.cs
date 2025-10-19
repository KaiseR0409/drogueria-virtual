using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace DrogueriaAPI.Models
{
    public class Proveedor
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // deja que EF maneje el Id
        public int IdProveedor { get; set; }

        public string? NombreProveedor { get; set; }

        public string? RUT { get; set; }                   // RUT tributario
        public string? Giro { get; set; }                  // Actividad económica (ej: “Comercialización de productos farmacéuticos”)
        public string? DireccionComercial { get; set; }    // Dirección donde emite la factura
        public string? Ciudad { get; set; }                // Ciudad o comuna



        // Clave foránea explícita hacia Usuario
        public int IdUsuario { get; set; }

        [JsonIgnore]
        [ForeignKey(nameof(IdUsuario))]
        public Usuarios Usuario { get; set; } = null!; // Relación 1:1 con Usuario
        [JsonIgnore]
        public virtual ICollection<ProveedorProducto> ProveedorProductos { get; set; } = new List<ProveedorProducto>();
    }
}
