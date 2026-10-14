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
        private readonly IWebHostEnvironment _environment;
        public TovarController(AppDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        // Вспомогательный метод для формирования полного URL к фото
        private string? GetPhotoUrl(string? photoFileName)
        {
            if (string.IsNullOrWhiteSpace(photoFileName))
                return null;

            return $"{Request.Scheme}://{Request.Host}/Images/{photoFileName}";
        }

        [HttpGet("AllProducts")]
        public async Task<IActionResult> AllProducts()
        {
            var rawProducts = await _context.Tovars
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

            var products = rawProducts.Select(t => new
            {
                t.Id,
                t.Article,
                t.Title,
                t.Price,
                t.Quantity,
                t.Description,
                Photo = GetPhotoUrl(t.Photo),
                t.Status,
                t.IdBrand,
                t.Brand,
                t.IdCategory,
                t.Category
            });

            return Ok(products);
        }

        [HttpGet("GetProduct/{id}")]
        public async Task<IActionResult> GetProduct(int id)
        {
            var t = await _context.Tovars
                .Include(p => p.IdBrandNavigation)
                .Include(p => p.IdCategoryNavigation)
                .Where(p => p.Id == id)
                .Select(p => new
                {
                    p.Id,
                    p.Article,
                    p.Title,
                    p.Price,
                    p.Quantity,
                    p.Description,
                    p.Photo,
                    p.Status,
                    p.IdBrand,
                    Brand = p.IdBrandNavigation != null ? p.IdBrandNavigation.Title : null,
                    p.IdCategory,
                    Category = p.IdCategoryNavigation != null ? p.IdCategoryNavigation.Title : null
                })
                .FirstOrDefaultAsync();

            if (t == null) return NotFound("Товар не найден.");

            var product = new
            {
                t.Id,
                t.Article,
                t.Title,
                t.Price,
                t.Quantity,
                t.Description,
                Photo = GetPhotoUrl(t.Photo),
                t.Status,
                t.IdBrand,
                t.Brand,
                t.IdCategory,
                t.Category
            };

            return Ok(product);
        }

        [HttpGet("Search")]
        public async Task<IActionResult> Search(string name)
        {
            var rawProducts = await _context.Tovars
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

            var products = rawProducts.Select(t => new
            {
                t.Id,
                t.Article,
                t.Title,
                t.Price,
                t.Quantity,
                t.Description,
                Photo = GetPhotoUrl(t.Photo),
                t.Brand,
                t.Category
            });

            return Ok(products);
        }

        [HttpGet("GetByCategory/{categoryId}")]
        public async Task<IActionResult> GetByCategory(int categoryId)
        {
            var rawProducts = await _context.Tovars
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

            var products = rawProducts.Select(t => new
            {
                t.Id,
                t.Article,
                t.Title,
                t.Price,
                t.Quantity,
                t.Description,
                Photo = GetPhotoUrl(t.Photo),
                t.Status,
                t.IdBrand,
                t.Brand,
                t.IdCategory,
                t.Category
            });

            return Ok(products);
        }

        [HttpGet("GetByBrand/{brandId}")]
        public async Task<IActionResult> GetByBrand(int brandId)
        {
            var rawProducts = await _context.Tovars
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

            var products = rawProducts.Select(t => new
            {
                t.Id,
                t.Article,
                t.Title,
                t.Price,
                t.Quantity,
                t.Description,
                Photo = GetPhotoUrl(t.Photo),
                t.Status,
                t.IdBrand,
                t.Brand,
                t.IdCategory,
                t.Category
            });

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

            var rawProducts = await query.Select(t => new
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

            var products = rawProducts.Select(t => new
            {
                t.Id,
                t.Article,
                t.Title,
                t.Price,
                t.Quantity,
                t.Description,
                Photo = GetPhotoUrl(t.Photo),
                t.Status,
                t.IdBrand,
                t.Brand,
                t.IdCategory,
                t.Category
            });

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

        [HttpPost("UploadPhoto")]
        public async Task<IActionResult> UploadPhoto(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("Файл не выбран");

            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "Images");
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            // Сохраняем с уникальным именем, чтобы имена не конфликтовали
            var extension = Path.GetExtension(file.FileName);
            var uniqueFileName = $"{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var fileUrl = $"{Request.Scheme}://{Request.Host}/Images/{uniqueFileName}";
            return Ok(new { fileName = uniqueFileName, url = fileUrl });
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
