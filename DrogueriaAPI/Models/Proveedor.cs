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

        // Clave foránea explícita hacia Usuario
        public int IdUsuario { get; set; }

        [JsonIgnore]
        [ForeignKey(nameof(IdUsuario))]
        public Usuarios Usuario { get; set; } = null!; // Relación 1:1 con Usuario
        [JsonIgnore]
        public virtual ICollection<ProveedorProducto> ProveedorProductos { get; set; } = new List<ProveedorProducto>();
    }
}
