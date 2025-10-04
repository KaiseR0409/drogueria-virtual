using System.ComponentModel.DataAnnotations;

namespace DrogueriaAPI.Models.DTOs
{
    public class AgregarUsuarioRequest
    {
        public string NombreUsuario { get; set; }
        public string Usuario { get; set; }
        public string Password { get; set; }
        public string TipoUsuario { get; set; }
        public string Correo { get; set; }
        public string Telefono { get; set; }  
    }
}
