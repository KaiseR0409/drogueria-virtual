namespace DrogueriaAPI.Models.DTOs
{
    public class OrdenDTO
    {
        public int IdOrden { get; set; }
        public DateTime FechaOrden { get; set; }
        public decimal MontoTotal { get; set; }
        public string NumeroFactura { get; set; }
        public DateTime? FechaFactura { get; set; }
        public int IdProveedor { get; set; }
        public List<ItemOrdenDTO> Items { get; set; }
    }
}
