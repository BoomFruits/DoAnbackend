
using DoAn.Data;
using DoAn.DTO;
using DoAn.Repositories;

namespace DoAn.Service
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryService(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }
       public async Task<List<CategoryDTO>> GetAllAsync()
        {
            var list = await _categoryRepository.GetAllAsync();
            return list.Select(c => new CategoryDTO { Id = c.Id, Name = c.Name }).ToList();
        }
        public async Task<CategoryDTO?> GetByIdAsync(int id)
        {
            var c = await _categoryRepository.GetByIdAsync(id);
            return c == null ? null : new CategoryDTO { Id = c.Id, Name = c.Name };
        }

        public async Task<CategoryDTO> CreateAsync(string name)
        {
            var category = new Category { Name = name };
            await _categoryRepository.AddAsync(category);
            await _categoryRepository.SaveChangesAsync();
            return new CategoryDTO { Id = category.Id, Name = category.Name };
        }

        public async Task<CategoryDTO?> UpdateAsync(int id, string newName)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null) return null;

            category.Name = newName;
            await _categoryRepository.UpdateAsync(category);
            await _categoryRepository.SaveChangesAsync();

            return new CategoryDTO { Id = category.Id, Name = category.Name };
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null) return false;

            await _categoryRepository.DeleteAsync(category);
            await _categoryRepository.SaveChangesAsync();
            return true;
        }
    }
}
