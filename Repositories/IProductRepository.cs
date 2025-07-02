using DoAn.Data;
using DoAn.DTO;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DoAn.Repositories
{
    public interface IProductRepository
    {
        Task<Product?> GetByIdAsync(int id);
        Task<IEnumerable<Product>> GetByCategoryAsync(int categoryId);
        Task<IEnumerable<Product>> GetAllAsync();
        Task<IEnumerable<Product>> GetAvailableProductsAsync();
        Task UpdateAsync(Product product);
        Task DeleteAsync(Product product);
        Task AddAsync(Product product);
        Task<PagedResult<Product>> FilterAsync(ProductFilterDTO filter);        
        Task SaveChangesAsync();

    }
}
