namespace DrogueriaAPI.Models.DTOs
{
    public class CrearOrdenRequest
    {
        public int IdUsuario { get; set; }
        public int IdProveedor { get; set; }
        public decimal MontoTotal { get; set; }

        // Campos de factura
        public string NumeroFactura { get; set; }
        public string TipoComprobante { get; set; }
        public DateTime? FechaFactura { get; set; }
        public string MetodoPago { get; set; }
        public string Moneda { get; set; }
        public decimal Impuestos { get; set; }
        public decimal Descuento { get; set; }
        public string DireccionEnvioCompleta { get; set; }

        // Items
        public List<ItemOrdenRequest> Items { get; set; }
    }

    public class ItemOrdenRequest
    {
        public int IdProducto { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal Impuesto { get; set; }
        public decimal Descuento { get; set; }
    }

}
