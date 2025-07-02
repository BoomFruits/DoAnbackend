using System.ComponentModel.DataAnnotations;

namespace DoAn.Data
{
    public class Product
    {
        public int Id { get; set; }
        [Required]
        public int CategoryId { get; set; }
        public Category Category { get; set; } = null!;
        [Required]
        [MaxLength(100)]
        public string Title { get; set; } = string.Empty;
        [MaxLength(1000)]
        public string Description { get; set; } = string.Empty;
        [Range(0, double.MaxValue, ErrorMessage = "Price must be non-negative")]
        public double Price { get; set; }
        [Range(0, int.MaxValue, ErrorMessage = "Amount must be non-negative")]
        public int StockQuantity { get; set; } // Số lượng sản phẩm có sẵn trong kho
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; } = null;
        public string Unit { get; set; } = string.Empty; // Đơn vị tính (ví dụ: cái, chai, kg, lít, ...)
        public bool IsAvailable { get; set; } = true; // Trạng thái sản phẩm có sẵn hay không
    }
}
