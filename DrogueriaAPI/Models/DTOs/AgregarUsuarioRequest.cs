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
        public string TipoEstablecimiento { get; set; }
        public string Direccion1 { get; set; }
        public string? Direccion2 { get; set; }
        public string? Direccion3 { get; set; }
        public string EstadoUsuario { get; set; }


        //Campos adicionales si es proveedor
        public string? NombreProveedor { get; set; }
        public string? Giro { get; set; }
        public string? DireccionComercial { get; set; }
        public string? Ciudad { get; set; }
        public string? Rut { get; set; }
    }
}
