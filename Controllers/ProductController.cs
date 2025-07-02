using DoAn.DTO;
using DoAn.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace DoAn.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ProductController : Controller
    {
        private readonly IProductService productService;
        public ProductController(IProductService productService)
        {
            this.productService = productService;
        }
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateProductDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var product = await productService.CreateProductAsync(dto);
            return Ok(product);
        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var products = await productService.GetAll();
            return Ok(products);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromBody] UpdateProductDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
       
            var updatedProduct = await productService.UpdateAsync(dto);
            if (updatedProduct == null)
            {
                return NotFound($"Product with ID {dto.Id} not found.");
            }
            return Ok(updatedProduct);
        }
        [HttpGet("Get_By_Category/{categoryId}")]
        public async Task<IActionResult> GetByCategory(int categoryId)
        {
            var products = await productService.GetByCategory(categoryId);
            if (products == null || !products.Any())
            {
                return NotFound($"No products found for category ID {categoryId}");
            }
            return Ok(products);
        }
        [HttpPost("filter")]
        public async Task<IActionResult> Filter([FromBody] ProductFilterDTO filter)
        {
            var result = await productService.FilterAsync(filter);
            return Ok(result);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await productService.DeleteAsync(id);
            if (!deleted)
            {
                return NotFound($"Product with ID {id} not found.");
            }
            return Ok(new { message = "Product deleted successfully." });
        }
    }
}
