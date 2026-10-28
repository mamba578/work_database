using API_project.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API_project.Controlles
{
    [Route("api/[controller]")]
    [ApiController]
    public class PointController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PointController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("AllPoints")]
        public ActionResult AllPoints()
        {
            var points = _context.Points.ToList();
            return Ok(points);
        }

        [HttpPost("AddPoint")]
        public ActionResult AddBrand(Point newPoint)
        {
            var currentPoints = _context.Points.FirstOrDefault(c => c.Address == newPoint.Address);
            if (currentPoints != null)
            {
                return BadRequest("Такая точка уже есть");
            }
            _context.Points.Add(newPoint);
            _context.SaveChanges();
            return Ok("Пункт выдачи добавлена");
        }
    }
}