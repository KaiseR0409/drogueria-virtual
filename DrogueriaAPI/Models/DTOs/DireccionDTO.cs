namespace DrogueriaAPI.Models.DTOs
{
    public class DireccionDTO
    {
        public int IdDireccion { get; set; }
        public int IdUsuario { get; set; }
        public string? Etiqueta { get; set; }
        public string Region { get; set; } = string.Empty;
        public string Comuna { get; set; } = string.Empty;
        public string Calle { get; set; } = string.Empty;
        public string NumeroCalle { get; set; } = string.Empty;
        public string? Complemento { get; set; }
        public bool EsPrincipal { get; set; }
        public string DireccionCompleta => $"{Calle} {NumeroCalle}, {Comuna}, {Region}";
    }
}
