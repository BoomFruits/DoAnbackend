using DoAn.DTO;

namespace DoAn.Service
{
    public interface ICategoryService
    {
        Task<List<CategoryDTO>> GetAllAsync();
        Task<CategoryDTO?> GetByIdAsync(int id);
        Task<CategoryDTO> CreateAsync(CategoryDTO dto);
        Task<CategoryDTO?> UpdateAsync(int id, CategoryDTO dto);
        Task<bool> DeleteAsync(int id);
    }
}
