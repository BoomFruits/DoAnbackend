using DoAn.Data;
using DoAn.DTO;

namespace DoAn.Service
{
    public interface IProductService
    {
        Task<IEnumerable<ProductDTO>> GetAll();
        Task<IEnumerable<ProductDTO>> GetByCategory(int id);
        Task<Product> CreateProductAsync(CreateProductDTO dto);
        Task<ProductDTO?> GetByIdAsync(int id);
        Task<ProductDTO?> UpdateAsync( UpdateProductDTO dto);
        Task<bool> DeleteAsync(int id);
        Task<PagedResult<ProductDTO>> FilterAsync(ProductFilterDTO filter);
    }
}
