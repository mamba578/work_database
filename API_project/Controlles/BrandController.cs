using API_project.Models;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace API_project.Controlles
{
    [Route("api/[controller]")]
    [ApiController]
    public class BrandController : ControllerBase
    {
        private readonly AppDbContext _context;

        public BrandController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("AllBrands")]
        public ActionResult AllBrands()
        {
            var brands = _context.Brands.ToList();
            return Ok(brands);
        }

        [HttpPost("AddBrand")]
        public ActionResult AddBrand(Brand newBrand)
        {
            var currentBrand = _context.Brands.FirstOrDefault(c => c.Title == newBrand.Title);
            if (currentBrand != null)
            {
                return BadRequest("Такая категория уже есть");
            }
            _context.Brands.Add(newBrand);
            _context.SaveChanges();
            return Ok("Категория добавлена");
        }
    }
}
