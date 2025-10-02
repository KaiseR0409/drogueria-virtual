using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace DrogueriaAPI.Models
{
    [Table ("Usuario")]
    public class Usuarios
    {
        [Key]
        public int IdUsuario { get; set; }
        public required string NombreUsuario { get; set; }
        public required string Usuario { get; set; }
        public required string Password { get; set; }
        public required string TipoUsuario { get; set; } // Admin o Usuario
        public DateTime? fechaCreacion { get; set; }
        public DateTime? fechaActualizacion { get; set; }
        public required string TipoEstablecimiento { get; set; } // Farmacia, Drogueria, Otro
        public required string direccion1 { get; set; }
        public string? direccion2 { get; set; }
        public string? direccion3 { get; set; }
        public required string EstadoUsuario { get; set; }

        // FIX: Agregar la propiedad de navegación Proveedor
        public Proveedor? Proveedor { get; set; }
    }
}
