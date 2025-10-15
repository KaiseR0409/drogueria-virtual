using System;
using System.ComponentModel.DataAnnotations;

public class ProductoPublicacionDto
{
    // Campos del Producto
    [Required]
    public string NombreProducto { get; set; }
    [Required]
    public string PrincipioActivo { get; set; }
    [Required]
    public string Concentracion { get; set; }
    [Required]
    public string FormaFarmaceutica { get; set; }
    [Required]
    public string PresentacionComercial { get; set; }
    [Required]
    public string LaboratorioFabricante { get; set; }
    [Required]
    public string RegistroSanitario { get; set; }

    public string? FechaVencimiento { get; set; }
    [Required]
    public string CondicionesAlmacenamiento { get; set; }

    // Nuevo campo para la URL de la imagen subida
    public string? ImagenUrl { get; set; }

    [Required]
    public string Marca { get; set; }

    [Required]
    [StringLength(12, ErrorMessage = "El código de barras debe tener como máximo 12 caracteres.")]
    public string CodigoBarras { get; set; }

    // Campos de Inventario (ProveedorProducto)
    [Required]
    [Range(0.01, double.MaxValue)]
    public decimal Precio { get; set; }
    [Required]
    [Range(0, int.MaxValue)]
    public int Stock { get; set; }
}