using DoAn.Data;
using DoAn.DTO;
using DoAn.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DoAn.Service
{
    public class ProductService : IProductService
    {
        private readonly DbBookingContext _context;
        private readonly IProductRepository _productRepo;
        private readonly ICategoryRepository _categoryRepo;
        public ProductService(DbBookingContext context, ICategoryRepository categoryRepo,IProductRepository productRepository)
        {
            _context = context;
            _categoryRepo = categoryRepo;
            _productRepo = productRepository;
        }

        public async Task<Product> CreateProductAsync(CreateProductDTO dto)
        {
            if (!await _categoryRepo.ExistsAsync(dto.CategoryId))
                throw new ArgumentException("Invalid CategoryId");

            var product = new Product
            {
                CategoryId = dto.CategoryId,
                Title = dto.Title,
                Description = dto.Description,
                Price = dto.Price,
                StockQuantity = dto.StockQuantity,
                Unit = dto.Unit,
                CreatedAt = DateTime.Now,
                IsAvailable = dto.StockQuantity > 0
            };

            await _productRepo.AddAsync(product);
            await _productRepo.SaveChangesAsync();

            return product;
        }
        public async Task<ProductDTO?> UpdateAsync(UpdateProductDTO dto)
        {
            var product = await _productRepo.GetByIdAsync(dto.Id);
            if (product == null) return null;

            product.Title = dto.Title;
            product.Description = dto.Description;
            product.Price = dto.Price;
            product.StockQuantity = dto.StockQuantity;
            product.Unit = dto.Unit;
            product.IsAvailable = dto.StockQuantity > 0;
            product.CategoryId = dto.CategoryId;
            product.UpdatedAt = DateTime.Now;

            await _productRepo.UpdateAsync(product);
            await _productRepo.SaveChangesAsync();

            return await GetByIdAsync(product.Id);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var product = await _productRepo.GetByIdAsync(id);
            if (product == null) return false;

            await _productRepo.DeleteAsync(product);
            await _productRepo.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<ProductDTO>> GetAll()
        {
            var products = await _productRepo.GetAllAsync();
            return products.Select(p => new ProductDTO
            {
                Id = p.Id,
                Title = p.Title,
                Description = p.Description,
                Price = p.Price,
                StockQuantity = p.StockQuantity,
                Unit = p.Unit,
                IsAvailable = p.IsAvailable,
                CategoryId = p.CategoryId,
                CategoryName = p.Category?.Name ?? "",
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt
            }).ToList();
        }

        public async Task<IEnumerable<ProductDTO>> GetByCategory(int id)
        {
            return await _context.Products
                .Where(p => p.CategoryId == id)
                .Include(p => p.Category)
                .Select(p => new ProductDTO
                {
                    Id = p.Id,
                    Title = p.Title,
                    CategoryName = p.Category.Name,
                    Price = p.Price,
                    StockQuantity = p.StockQuantity,
                    Unit = p.Unit,
                    IsAvailable = p.IsAvailable
                })
                .ToListAsync()
                .ConfigureAwait(false);
        }
        public async Task<PagedResult<ProductDTO>> FilterAsync(ProductFilterDTO filter)
        {
            var result = await _productRepo.FilterAsync(filter);

            return new PagedResult<ProductDTO>
            {
                Items = result.Items.Select(p => new ProductDTO
                {
                    Id = p.Id,
                    Title = p.Title,
                    Description = p.Description,
                    Price = p.Price,
                    StockQuantity = p.StockQuantity,
                    Unit = p.Unit,
                    IsAvailable = p.IsAvailable,
                    CategoryId = p.CategoryId,
                    CategoryName = p.Category?.Name ?? ""
                }).ToList(),
                TotalItems = result.TotalItems,
                Page = result.Page,
                PageSize = result.PageSize
            };
        }
        public Task<ProductDTO?> GetByIdAsync(int id)
        {
            return _context.Products
                .Where(p => p.Id == id)
                .Include(p => p.Category)
                .Select(p => new ProductDTO
                {
                    Id = p.Id,
                    Title = p.Title,
                    CategoryName = p.Category.Name,
                    Price = p.Price,
                    StockQuantity = p.StockQuantity,
                    Unit = p.Unit,
                    IsAvailable = p.IsAvailable
                })
                .FirstOrDefaultAsync();
        }
    }
}
