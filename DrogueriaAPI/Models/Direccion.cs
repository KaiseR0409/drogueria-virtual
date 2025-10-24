using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DrogueriaAPI.Models
{
    public class Direccion
    {
        [Key]
        public int IdDireccion { get; set; }

        [Required]
        [ForeignKey("Usuarios")]
        public int IdUsuario { get; set; }

        [StringLength(50)]
        public string? Etiqueta { get; set; } // Ej: Casa, Trabajo

        [Required]
        [StringLength(100)]
        public string Region { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Comuna { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Calle { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string NumeroCalle { get; set; } = string.Empty;

        [StringLength(255)]
        public string? Complemento { get; set; } // Ej: Depto 302, Oficina, etc.

        public bool EsPrincipal { get; set; } = false;

        // Relación con Usuario
        public Usuarios? Usuario { get; set; }

        // Propiedad calculada (no se guarda en BD)
        [NotMapped]
        public string DireccionCompleta => $"{Calle} {NumeroCalle}, {Comuna}, {Region}";
    }
}

