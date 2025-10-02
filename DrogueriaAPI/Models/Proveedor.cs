using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
namespace DrogueriaAPI.Models
{
    public class Proveedor
    {
        [Key]
        
        public int IdProveedor { get; set; }
        public string? NombreProveedor { get; set; }


        [JsonIgnore]
        // Relación con Usuario
        public virtual Usuarios? Usuario { get; set; }
    }
}

