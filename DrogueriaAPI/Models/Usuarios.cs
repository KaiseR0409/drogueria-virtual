using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace DrogueriaAPI.Models
{
    [Table ("Usuario")]
    public class Usuarios
    {
        [Key]
        public int IdUsuario { get; set; }
        public string NombreUsuario { get; set; }      // Nombre completo
        public string Usuario { get; set; }           // Nombre de login
        public string? Password { get; set;}
        public string TipoUsuario { get; set; }       // Cliente / Proveedor / Administrador

        public DateTime FechaCreacion { get; set; }
        public DateTime FechaActualizacion { get; set; }

        public string TipoEstablecimiento { get; set; } // Ej: Cliente, Farmacia, etc.

        public string Direccion1 { get; set; }
        public string? Direccion2 { get; set; }
        public string? Direccion3 { get; set; }

        public string EstadoUsuario { get; set; }     // Activo / Inactivo

        public string Correo { get; set; }
        public string Telefono { get; set; }
        public Proveedor? Proveedor { get; set; }

        [InverseProperty("Usuario")]
        public virtual ICollection<Orden> Ordenes { get; set; } = new List<Orden>();
    }
}
