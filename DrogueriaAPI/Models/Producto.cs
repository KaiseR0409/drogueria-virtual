using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace DrogueriaAPI.Models
{
    // Usar el atributo [Table] para especificar el nombre de la tabla en la BD
    [Table("Producto")]
    public class Producto
    {
        [Key]
        public int IdProducto { get; set; }
        [Required]
        public string Marca { get; set; }

        [Required]
        [StringLength(12, ErrorMessage = "El código de barras debe tener como máximo 12 caracteres.")]
        public string CodigoBarras { get; set; }
        public required string NombreProducto { get; set; }
        public required string PrincipioActivo { get; set; }
        public required string Concentracion { get; set; }
        public required string FormaFarmaceutica { get; set; }
        public required string PresentacionComercial { get; set; }
        public required string LaboratorioFabricante { get; set; }
        public required string RegistroSanitario { get; set; }
        public DateTime? FechaVencimiento { get; set; }
        public required string CondicionesAlmacenamiento { get; set; }
        public string? ImagenUrl { get; set; }

        // Propiedades de Navegación (para ProveedorProducto)
        public ICollection<ProveedorProducto> InventarioProveedores { get; set; } = new List<ProveedorProducto>();

        // Propiedades de Navegación (para ItemOrden)
        public ICollection<ItemOrden> ItemsOrden { get; set; } = new List<ItemOrden>();
    }
}
