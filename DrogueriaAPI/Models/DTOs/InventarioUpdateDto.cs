using System.ComponentModel.DataAnnotations;

namespace DrogueriaAPI.Models.DTOs
{
    public class InventarioUpdateDto
    {
        [Range(0.01, double.MaxValue)]
        public decimal? Precio { get; set; }
        [Range(0, int.MaxValue)]
        public int? Stock { get; set; }
    }
}
