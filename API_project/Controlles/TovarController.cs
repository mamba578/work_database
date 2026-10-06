using API_project.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace API_project.Controlles
{
    [Route("api/[controller]")]
    [ApiController]
    public class TovarController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TovarController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("AllProducts")]
        public async Task<IActionResult> AllProducts()
        {
            var products = await _context.Tovars
                .Include(t => t.IdBrandNavigation)
                .Include(t => t.IdCategoryNavigation)
                .Where(t => t.Status == "Активен")
                .Select(t => new
                {
                    t.Id,
                    t.Article,
                    t.Title,
                    t.Price,
                    t.Quantity,
                    t.Description,
                    t.Photo,
                    t.Status,
                    t.IdBrand,
                    Brand = t.IdBrandNavigation != null ? t.IdBrandNavigation.Title : null,
                    t.IdCategory,
                    Category = t.IdCategoryNavigation != null ? t.IdCategoryNavigation.Title : null
                })
                .ToListAsync();

            return Ok(products);
        }

        [HttpGet("GetProduct/{id}")]
        public async Task<IActionResult> GetProduct(int id)
        {
            var product = await _context.Tovars
                .Include(t => t.IdBrandNavigation)
                .Include(t => t.IdCategoryNavigation)
                .Where(t => t.Id == id)
                .Select(t => new
                {
                    t.Id,
                    t.Article,
                    t.Title,
                    t.Price,
                    t.Quantity,
                    t.Description,
                    t.Photo,
                    t.Status,
                    t.IdBrand,
                    Brand = t.IdBrandNavigation != null ? t.IdBrandNavigation.Title : null,
                    t.IdCategory,
                    Category = t.IdCategoryNavigation != null ? t.IdCategoryNavigation.Title : null
                })
                .FirstOrDefaultAsync();

            return product == null ? NotFound("Товар не найден.") : Ok(product);
        }

        [HttpGet("Search")]
        public async Task<IActionResult> Search(string name)
        {
            var products = await _context.Tovars
                .Include(t => t.IdBrandNavigation)
                .Include(t => t.IdCategoryNavigation)
                .Where(t => (t.Title.Contains(name) || t.Article.Contains(name)) && t.Status == "Активен")
                .Select(t => new
                {
                    t.Id,
                    t.Article,
                    t.Title,
                    t.Price,
                    t.Quantity,
                    t.Description,
                    t.Photo,
                    Brand = t.IdBrandNavigation != null ? t.IdBrandNavigation.Title : null,
                    Category = t.IdCategoryNavigation != null ? t.IdCategoryNavigation.Title : null
                })
                .ToListAsync();

            return Ok(products);
        }

        [HttpGet("GetByCategory/{categoryId}")]
        public async Task<IActionResult> GetByCategory(int categoryId)
        {
            var products = await _context.Tovars
                .Include(t => t.IdBrandNavigation)
                .Include(t => t.IdCategoryNavigation)
                .Where(t => t.IdCategory == categoryId && t.Status == "Активен")
                .Select(t => new
                {
                    t.Id,
                    t.Article,
                    t.Title,
                    t.Price,
                    t.Quantity,
                    t.Description,
                    t.Photo,
                    t.Status,
                    t.IdBrand,
                    Brand = t.IdBrandNavigation != null ? t.IdBrandNavigation.Title : null,
                    t.IdCategory,
                    Category = t.IdCategoryNavigation != null ? t.IdCategoryNavigation.Title : null
                })
                .ToListAsync();

            return Ok(products);
        }

        [HttpGet("GetByBrand/{brandId}")]
        public async Task<IActionResult> GetByBrand(int brandId)
        {
            var products = await _context.Tovars
                .Include(t => t.IdBrandNavigation)
                .Include(t => t.IdCategoryNavigation)
                .Where(t => t.IdBrand == brandId && t.Status == "Активен")
                .Select(t => new
                {
                    t.Id,
                    t.Article,
                    t.Title,
                    t.Price,
                    t.Quantity,
                    t.Description,
                    t.Photo,
                    t.Status,
                    t.IdBrand,
                    Brand = t.IdBrandNavigation != null ? t.IdBrandNavigation.Title : null,
                    t.IdCategory,
                    Category = t.IdCategoryNavigation != null ? t.IdCategoryNavigation.Title : null
                })
                .ToListAsync();

            return Ok(products);
        }

        [HttpGet("Filter")]
        public async Task<IActionResult> Filter([FromQuery] int? categoryId, [FromQuery] int? brandId, [FromQuery] string? search)
        {
            var query = _context.Tovars
                .Include(t => t.IdBrandNavigation)
                .Include(t => t.IdCategoryNavigation)
                .Where(t => t.Status == "Активен");

            if (categoryId.HasValue)
            {
                query = query.Where(t => t.IdCategory == categoryId.Value);
            }

            if (brandId.HasValue)
            {
                query = query.Where(t => t.IdBrand == brandId.Value);
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(t => t.Title.Contains(search) || t.Article.Contains(search));
            }

            var products = await query.Select(t => new
            {
                t.Id,
                t.Article,
                t.Title,
                t.Price,
                t.Quantity,
                t.Description,
                t.Photo,
                t.Status,
                t.IdBrand,
                Brand = t.IdBrandNavigation != null ? t.IdBrandNavigation.Title : null,
                t.IdCategory,
                Category = t.IdCategoryNavigation != null ? t.IdCategoryNavigation.Title : null
            }).ToListAsync();

            return Ok(products);
        }

        [HttpPost("AddProduct")]
        public async Task<IActionResult> AddProduct([FromBody] Tovar newProduct)
        {
            try
            {
                newProduct.Id = 0;
                newProduct.IdBrandNavigation = null;
                newProduct.IdCategoryNavigation = null;

                if (string.IsNullOrEmpty(newProduct.Status))
                {
                    newProduct.Status = "Активен";
                }

                _context.Tovars.Add(newProduct);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Товар успешно добавлен", productId = newProduct.Id });
            }
            catch (Exception ex)
            {
                var inner = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return StatusCode(500, $"Ошибка при добавлении в БД: {inner}");
            }
        }

        [HttpPut("UpdateProduct/{id}")]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] Tovar product)
        {
            try
            {
                var oldProduct = await _context.Tovars.FirstOrDefaultAsync(p => p.Id == id);
                if (oldProduct == null) return NotFound("Товар не найден");

                oldProduct.Article = product.Article;
                oldProduct.Title = product.Title;
                oldProduct.Price = product.Price;
                oldProduct.IdCategory = product.IdCategory;
                oldProduct.IdBrand = product.IdBrand;
                oldProduct.Description = product.Description;
                oldProduct.Quantity = product.Quantity;
                oldProduct.Status = product.Status;
                oldProduct.Photo = product.Photo;

                await _context.SaveChangesAsync();
                return Ok(oldProduct);
            }
            catch (Exception ex)
            {
                var inner = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return StatusCode(500, $"Ошибка при обновлении в БД: {inner}");
            }
        }

        [HttpDelete("DeleteProduct/{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _context.Tovars.FirstOrDefaultAsync(p => p.Id == id);
            if (product == null) return NotFound("Товар не найден");

            product.Status = "Удален";
            await _context.SaveChangesAsync();

            return Ok("Товар переведен в статус 'Удален'");
        }
    }
}
