using API_project.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace API_project.Controlles
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UserController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("AllUsers")]
        public async Task<IActionResult> AllUsers()
        {
            var users = await _context.Users
                .Select(u => new
                {
                    u.Id,
                    u.Login,
                    u.Name,
                    u.Surname,
                    u.Patronymic,
                    u.Email,
                    u.Status,
                    u.Role
                })
                .ToListAsync();

            return Ok(users);
        }

        [HttpGet("GetProfile/{id}")]
        public async Task<IActionResult> GetProfile(int id)
        {
            var user = await _context.Users
                .Where(u => u.Id == id)
                .Select(u => new
                {
                    u.Id,
                    u.Login,
                    u.Name,
                    u.Surname,
                    u.Patronymic,
                    u.Email,
                    u.Status,
                    u.Role
                })
                .FirstOrDefaultAsync();

            return user == null ? NotFound("Пользователь не найден.") : Ok(user);
        }

        [HttpPost("Authorization")]
        public ActionResult Authorization([FromQuery] string login, [FromQuery] string password)
        {
            var currentUser = _context.Users.FirstOrDefault(u => u.Login == login && u.Password == password);
            return currentUser == null ? NotFound("Неверный логин или пароль.") : Ok(currentUser);
        }

        [HttpPost("Registration")]
        public async Task<IActionResult> Registration([FromBody] User newUser)
        {
            try
            {
                var exists = await _context.Users.AnyAsync(u =>
                    u.Login.ToLower() == newUser.Login.ToLower() ||
                    u.Email.ToLower() == newUser.Email.ToLower());

                if (exists)
                {
                    return BadRequest("Пользователь с таким логином или почтой уже существует.");
                }

                newUser.Role = "Клиент";
                newUser.Status = "Активный";

                _context.Users.Add(newUser);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Пользователь успешно зарегистрирован", userId = newUser.Id });
            }
            catch (Exception ex)
            {
                var inner = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return StatusCode(500, $"Ошибка базы данных при регистрации: {inner}");
            }
        }

        [HttpPut("UpdateUser")]
        public async Task<ActionResult> UpdateUser(int id, [FromBody] User updatedUser)
        {
            var dbUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);

            if (dbUser == null)
            {
                return NotFound($"Пользователь с ID {id} не найден");
            }

            dbUser.Name = updatedUser.Name;
            dbUser.Surname = updatedUser.Surname;
            dbUser.Patronymic = updatedUser.Patronymic;
            dbUser.Login = updatedUser.Login;
            dbUser.Password = updatedUser.Password;
            dbUser.Email = updatedUser.Email;

            await _context.SaveChangesAsync();

            return Ok(dbUser);
        }

        [HttpPut("DeleteAccount")]
        public ActionResult DeleteAccount(int deleteId)
        {
            var currentUser = _context.Users.FirstOrDefault(u => u.Id == deleteId);
            if (currentUser == null)
            {
                return BadRequest("Аккаунт пользователя не найден");
            }
            currentUser.Status = "Неактивный";
            _context.SaveChanges();
            return Ok("Аккаунт удален");
        }
    }
}
