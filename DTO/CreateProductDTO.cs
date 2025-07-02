using System.ComponentModel.DataAnnotations;

namespace DoAn.DTO
{
    public class CreateProductDTO
    {
        [Required]
        public int CategoryId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Title { get; set; } = string.Empty;

        public IFormFile? Thumbnail { get; set; }

        [MaxLength(1000)]
        public string Description { get; set; } = string.Empty;

        [Range(0, double.MaxValue)]
        public double Price { get; set; }

        [Range(0, int.MaxValue)]
        public int StockQuantity { get; set; }

        public string Unit { get; set; } = string.Empty;
    }
}
