namespace DrogueriaAPI.Models.DTOs
{
    public class UsuarioRegistroDto
    {
        // Datos del usuario
        public string Usuario { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string NombreUsuario { get; set; } = string.Empty;
        public string TipoUsuario { get; set; } = string.Empty; // Cliente o Proveedor
        public string? Correo { get; set; }
        public string? Telefono { get; set; }

        // Datos de proveedor (solo se usarán si TipoUsuario == "Proveedor")
        public string? NombreProveedor { get; set; }
        public string? Rut { get; set; }
        public string? Giro { get; set; }
        public string? DireccionComercial { get; set; }
        public string? Ciudad { get; set; }
    }
}
