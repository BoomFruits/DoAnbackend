using System.ComponentModel.DataAnnotations;

namespace DoAn.DTO
{
    public class UpdateProductDTO : CreateProductDTO
    {
        [Required]
        public int Id { get; set; }
    }
}
