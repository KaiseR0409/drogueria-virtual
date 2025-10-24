namespace DrogueriaAPI.Models.DTOs
{
    public class CrearOrdenRequest
    {
        // --- Datos básicos de la orden ---
        public int IdUsuario { get; set; }
        public int IdProveedor { get; set; }

        public decimal? MontoTotal { get; set; } = null;

        // --- Datos de factura (opcionales al crear la orden) ---
        public string? NumeroFactura { get; set; } = null;
        public string TipoComprobante { get; set; } = "Boleta";
        public DateTime? FechaFactura { get; set; } = null;
        public string MetodoPago { get; set; } = "Pendiente";
        public string Moneda { get; set; } = "CLP";

        public decimal Impuestos { get; set; } = 0;
        public decimal Descuento { get; set; } = 0;

        // --- Lista de ítems ---
        public List<ItemOrdenRequest> Items { get; set; } = new();

        public int IdDireccion { get; set; }
    }

    public class ItemOrdenRequest
    {
        public int IdProducto { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal Descuento { get; set; } = 0;
    }
}
