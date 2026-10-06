using API_project.Models;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace API_project.Controlles
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CategoryController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("AllCategories")]
        public ActionResult AllCategories()
        {
            var brands = _context.Categories.ToList();
            return Ok(brands);
        }

        [HttpPost("AddCategory")]
        public ActionResult AddCategory(Category newCategory)
        {
            var currentCategory = _context.Categories.FirstOrDefault(c => c.Title == newCategory.Title);
            if(currentCategory != null)
            {
                return BadRequest("Такая категория уже есть");
            }
            _context.Categories.Add(newCategory);
            _context.SaveChanges();
            return Ok("Категория добавлена");
        }
    }
}
