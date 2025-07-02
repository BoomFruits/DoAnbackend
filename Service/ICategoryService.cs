using DoAn.DTO;

namespace DoAn.Service
{
    public interface ICategoryService
    {
        Task<List<CategoryDTO>> GetAllAsync();
        Task<CategoryDTO?> GetByIdAsync(int id);
        Task<CategoryDTO> CreateAsync(string name);
        Task<CategoryDTO?> UpdateAsync(int id, string newName);
        Task<bool> DeleteAsync(int id);
    }
}
