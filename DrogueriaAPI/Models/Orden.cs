using DrogueriaAPI.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Orden
{
    [Key]
    public int IdOrden { get; set; }
    [MaxLength(500)]
    public string? DireccionEnvioCompleta { get; set; }


    // Relación con el comprador
    public int IdUsuario { get; set; }

    // Relación con el proveedor
    public int IdProveedor { get; set; }
    [ForeignKey("IdProveedor")] 
    public virtual Proveedor Proveedor { get; set; }

    // Datos de la orden
    public DateTime FechaOrden { get; set; }
    public string EstadoOrden { get; set; } // Pendiente, Pagada, Cancelada, Enviada
    public decimal MontoTotal { get; set; }

    // Datos para escalar a factura
    public string? NumeroOrdenCompra { get; set; } // OC-00001
    public string NumeroFactura { get; set; } // FAC-00001 (cuando se emita la factura)
    public string TipoComprobante { get; set; } // Factura, Boleta, Nota de crédito
    public DateTime? FechaFactura { get; set; }
    public string MetodoPago { get; set; } // Transferencia, Tarjeta, Efectivo
    public string Moneda { get; set; } = "CLP";
    public decimal Impuestos { get; set; } // IVA / IGV / etc.
    public decimal Descuento { get; set; }


    // Relación con los ítems
    public ICollection<ItemOrden> Items { get; set; }
}