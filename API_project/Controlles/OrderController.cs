using System.Net;
using System.Net.Mail;
using System.Text;
using API_project.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace API_project.Controlles
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly AppDbContext _context;
        
        public OrderController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("AllOrders")]
        public async Task<IActionResult> AllOrders()
        {
            var orders = await _context.Orders
                .Include(o => o.IdUserNavigation)
                .Include(o => o.IdStatusNavigation)
                .Include(o => o.IdPointNavigation)
                .OrderByDescending(o => o.OrderDate)
                .Select(o => new
                {
                    o.Id,
                    o.OrderDate,
                    o.DeliveryDate,
                    o.TotalPrice,
                    o.PaymentType,
                    o.IdStatus,
                    Status = o.IdStatusNavigation != null ? o.IdStatusNavigation.Title : null,
                    o.IdUser,
                    UserName = o.IdUserNavigation != null ? $"{o.IdUserNavigation.Surname} {o.IdUserNavigation.Name}" : null,
                    o.IdPoint,
                    PointAddress = o.IdPointNavigation != null ? $"{o.IdPointNavigation.City}, {o.IdPointNavigation.Street} {o.IdPointNavigation.HomeNumber}" : null
                })
                .ToListAsync();

            return Ok(orders);
        }

        [HttpGet("UserOrders/{userId}")]
        public async Task<IActionResult> GetUserOrders(int userId)
        {
            var orders = await _context.Orders
                .Include(o => o.IdStatusNavigation)
                .Include(o => o.IdPointNavigation)
                .Where(o => o.IdUser == userId)
                .OrderByDescending(o => o.OrderDate)
                .Select(o => new
                {
                    o.Id,
                    o.OrderDate,
                    o.TotalPrice,
                    o.PaymentType,
                    o.IdStatus,
                    Status = o.IdStatusNavigation != null ? o.IdStatusNavigation.Title : null,
                    o.IdPoint,
                    PointAddress = o.IdPointNavigation != null ? $"{o.IdPointNavigation.City}, {o.IdPointNavigation.Street} {o.IdPointNavigation.HomeNumber}" : null
                })
                .ToListAsync();

            return Ok(orders);
        }

        [HttpGet("GetActiveOrder/{userId}")]
        public async Task<IActionResult> GetActiveOrder(int userId)
        {
            const int cartStatusId = 1;

            var order = await _context.Orders
                .FirstOrDefaultAsync(o => o.IdUser == userId && o.IdStatus == cartStatusId);

            if (order == null)
            {
                order = new Order
                {
                    IdUser = userId,
                    OrderDate = DateTime.Now,
                    TotalPrice = 0,
                    IdStatus = cartStatusId,
                    IdPoint = 1,
                    PaymentType = "При получении"
                };

                _context.Orders.Add(order);
                await _context.SaveChangesAsync();
            }

            return Ok(new
            {
                order.Id,
                order.IdUser,
                order.OrderDate,
                order.TotalPrice,
                order.IdStatus,
                order.IdPoint,
                order.PaymentType
            });
        }

        [HttpPost("CreateOrder")]
        public async Task<IActionResult> CreateOrder([FromBody] Order newOrder)
        {
            try
            {
                newOrder.Id = 0;
                newOrder.IdUserNavigation = null!;
                newOrder.IdStatusNavigation = null!;
                newOrder.IdPointNavigation = null!;

                if (newOrder.OrderDate == default)
                {
                    newOrder.OrderDate = DateTime.Now;
                }

                if (newOrder.IdStatus == 0) newOrder.IdStatus = 1;
                if (newOrder.IdPoint == 0) newOrder.IdPoint = 1;

                _context.Orders.Add(newOrder);
                await _context.SaveChangesAsync();

                return Ok(newOrder);
            }
            catch (Exception ex)
            {
                var inner = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return StatusCode(500, $"Ошибка при создании заказа: {inner}");
            }
        }

       [HttpPut("UpdateStatus/{idOrder}")]
        public async Task<IActionResult> UpdateStatus(
            int idOrder,
            [FromQuery] int newStatusId,
            [FromQuery] int idPoint = 0,
            [FromQuery] string? paymentType = null,
            [FromQuery] string? email = null)
        {
            try
            {
                var oldOrder = await _context.Orders.FirstOrDefaultAsync(x => x.Id == idOrder);

                if (oldOrder == null)
                    return NotFound("Заказ не найден в базе данных");

                const int draftStatusId = 1;
                const int paidStatusId = 2;

                var itemsInCart = await _context.OrderItems
                    .Include(oi => oi.IdTovarNavigation)
                    .Where(oi => oi.IdOrder == oldOrder.Id)
                    .ToListAsync();

                if (newStatusId == paidStatusId && oldOrder.IdStatus == draftStatusId)
                {
                    if (!itemsInCart.Any())
                    {
                        return BadRequest("Невозможно оформить пустой заказ (нет товаров в корзине)");
                    }

                    foreach (var item in itemsInCart)
                    {
                        var product = await _context.Tovars.FirstOrDefaultAsync(p => p.Id == item.IdTovar);

                        if (product != null)
                        {
                            if (product.Quantity < item.Quantity)
                            {
                                return BadRequest($"Недостаточно товара '{product.Title}' на складе. Доступно: {product.Quantity}, требуется: {item.Quantity}");
                            }

                            product.Quantity -= item.Quantity;
                        }
                    }
                }

                oldOrder.IdStatus = newStatusId;
                if (idPoint > 0) oldOrder.IdPoint = idPoint;
                if (!string.IsNullOrEmpty(paymentType)) oldOrder.PaymentType = paymentType;

                await _context.SaveChangesAsync();

                // РЕАЛЬНАЯ ОТПРАВКА ЧЕКА СТРОГО НА ПОЧТУ КЛИЕНТА
                string? targetEmail = email;
                if (string.IsNullOrEmpty(targetEmail))
                {
                    var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == oldOrder.IdUser);
                    targetEmail = user?.Email;
                }

                if (!string.IsNullOrEmpty(targetEmail))
                {
                    try
                    {
                        // Настройки SMTP-сервера почты отправителя (магазина)
                        var smtpClient = new SmtpClient("smtp.yandex.ru")
                        {
                            Port = 587,
                            Credentials = new NetworkCredential("admin@artstore.ru", "пароль_приложения_почты_магазина"),
                            EnableSsl = true,
                        };

                        var mailMessage = new MailMessage
                        {
                            From = new MailAddress("admin@artstore.ru", "Интернет-магазин Azure"),
                            Subject = $"Электронный чек к заказу #{oldOrder.Id}",
                            IsBodyHtml = true,
                        };
                        
                        // Письмо уходит строго на личную почту клиента
                        mailMessage.To.Add(targetEmail);

                        // Формирование HTML-чека
                        var sb = new StringBuilder();
                        sb.Append($"<h2>Спасибо за покупку в интернет-магазине «Azure»!</h2>");
                        sb.Append($"<p>Ваш заказ <b>#{oldOrder.Id}</b> успешно оформлен и оплачен.</p>");
                        sb.Append($"<p><b>Способ оплаты:</b> {oldOrder.PaymentType}</p>");
                        sb.Append($"<h3>Состав заказа:</h3><ul>");
                        
                        foreach (var i in itemsInCart)
                        {
                            var title = i.IdTovarNavigation?.Title ?? $"Товар #{i.IdTovar}";
                            sb.Append($"<li>{title} — {i.Quantity} шт. по {i.Price} ₽</li>");
                        }
                        
                        sb.Append($"</ul>");
                        sb.Append($"<h3>Итого к оплате: {oldOrder.TotalPrice} ₽</h3>");

                        mailMessage.Body = sb.ToString();

                        await smtpClient.SendMailAsync(mailMessage);
                    }
                    catch (Exception mailEx)
                    {
                        Console.WriteLine($"Ошибка отправки почты клиенту: {mailEx.Message}");
                    }
                }

                return Ok("Заказ успешно обновлен и чек отправлен.");
            }
            catch (Exception ex)
            {
                var inner = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return StatusCode(500, $"Ошибка сервера: {inner}");
            }
        }

    } 
}
