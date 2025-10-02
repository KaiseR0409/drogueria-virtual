namespace DrogueriaAPI.Models.DTOs
{
    public class ConfirmarPagoRequest
    {
        public string NumeroFactura { get; set; } = string.Empty;
        public string TipoComprobante { get; set; } = "Factura Electrónica";
        public string MetodoPago { get; set; } = string.Empty;
        public string Moneda { get; set; } = "CLP";
        public decimal Impuestos { get; set; }
        public decimal Descuento { get; set; }
    }
}
