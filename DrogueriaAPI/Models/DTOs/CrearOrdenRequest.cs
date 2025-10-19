namespace DrogueriaAPI.Models.DTOs
{
    public class CrearOrdenRequest
    {
        // --- Datos básicos de la orden ---
        public int IdUsuario { get; set; }
        public int IdProveedor { get; set; }
        public decimal MontoTotal { get; set; }

        // --- Datos de factura (opcionales al crear la orden) ---
        // Estos campos se pueden completar al confirmar el pago.
        public string? NumeroFactura { get; set; } = null;
        public string TipoComprobante { get; set; } = "Boleta"; // valor por defecto
        public DateTime? FechaFactura { get; set; } = null;
        public string MetodoPago { get; set; } = "Pendiente";   // por defecto
        public string Moneda { get; set; } = "CLP";
        public decimal Impuestos { get; set; } = 0;
        public decimal Descuento { get; set; } = 0;

        // --- Dirección de envío ---
        public string DireccionEnvioCompleta { get; set; }

        // --- Lista de ítems ---
        public List<ItemOrdenRequest> Items { get; set; } = new();
    }

    public class ItemOrdenRequest
    {
        public int IdProducto { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal Impuesto { get; set; } = 0;
        public decimal Descuento { get; set; } = 0;
    }
}
