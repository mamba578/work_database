using API_project.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace API_project.Controlles
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderItemsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public OrderItemsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("AllItems")]
        public async Task<IActionResult> AllItems()
        {
            var items = await _context.OrderItems
                .Include(oi => oi.IdTovarNavigation)
                .Select(oi => new
                {
                    oi.Id,
                    oi.IdOrder,
                    oi.IdTovar,
                    TovarName = oi.IdTovarNavigation != null ? oi.IdTovarNavigation.Title : null,
                    oi.Quantity,
                    oi.Price
                })
                .ToListAsync();

            return Ok(items);
        }

        [HttpGet("ByOrder/{orderId}")]
        public async Task<IActionResult> GetItemsByOrder(int orderId)
        {
            var items = await _context.OrderItems
                .Include(oi => oi.IdTovarNavigation)
                .Where(oi => oi.IdOrder == orderId)
                .Select(oi => new
                {
                    oi.Id,
                    oi.IdOrder,
                    oi.IdTovar,
                    TovarName = oi.IdTovarNavigation != null ? oi.IdTovarNavigation.Title : null,
                    TovarPhoto = oi.IdTovarNavigation != null ? oi.IdTovarNavigation.Photo : null,
                    oi.Quantity,
                    oi.Price
                })
                .ToListAsync();

            return Ok(items);
        }

        [HttpPost("AddItem")]
        public async Task<IActionResult> AddItem([FromBody] OrderItem newItem)
        {
            try
            {
                newItem.Id = 0;
                newItem.IdOrderNavigation = null!;
                newItem.IdTovarNavigation = null!;

                var tovar = await _context.Tovars.FirstOrDefaultAsync(t => t.Id == newItem.IdTovar);
                if (tovar == null)
                {
                    return NotFound("Товар не найден в базе данных");
                }

                newItem.Price = tovar.Price;

                var orderExists = await _context.Orders.AnyAsync(o => o.Id == newItem.IdOrder);

                if (newItem.IdOrder == 0 || !orderExists)
                {
                    var newOrder = new Order
                    {
                        IdUser = 1,
                        OrderDate = DateTime.Now,
                        TotalPrice = 0,
                        IdStatus = 1,
                        IdPoint = 1,
                        PaymentType = "При получении"
                    };

                    _context.Orders.Add(newOrder);
                    await _context.SaveChangesAsync();
                    newItem.IdOrder = newOrder.Id;
                }

                var existingItem = await _context.OrderItems
                    .FirstOrDefaultAsync(oi => oi.IdOrder == newItem.IdOrder && oi.IdTovar == newItem.IdTovar);

                if (existingItem != null)
                {
                    existingItem.Quantity += newItem.Quantity > 0 ? newItem.Quantity : 1;
                    existingItem.Price = tovar.Price;
                }
                else
                {
                    if (newItem.Quantity <= 0) newItem.Quantity = 1;
                    _context.OrderItems.Add(newItem);
                }

                await _context.SaveChangesAsync();

                await UpdateOrderTotalPrice(newItem.IdOrder);

                return Ok(new { message = "Товар успешно добавлен в корзину", orderId = newItem.IdOrder });
            }
            catch (Exception ex)
            {
                var innerMessage = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return StatusCode(500, $"Ошибка БД: {innerMessage}");
            }
        }

        [HttpPut("UpdateQuantity/{id}")]
        public async Task<IActionResult> UpdateQuantity(int id, int quantity)
        {
            var item = await _context.OrderItems.FirstOrDefaultAsync(oi => oi.Id == id);
            if (item == null) return NotFound("Позиция не найдена");

            if (quantity <= 0)
            {
                _context.OrderItems.Remove(item);
            }
            else
            {
                item.Quantity = quantity;
            }

            await _context.SaveChangesAsync();

            await UpdateOrderTotalPrice(item.IdOrder);

            return Ok("Количество обновлено");
        }

        [HttpDelete("DeleteItem/{id}")]
        public async Task<IActionResult> DeleteItem(int id)
        {
            var item = await _context.OrderItems.FirstOrDefaultAsync(oi => oi.Id == id);
            if (item == null) return NotFound("Позиция не найдена");

            int orderId = item.IdOrder;
            _context.OrderItems.Remove(item);
            await _context.SaveChangesAsync();

            await UpdateOrderTotalPrice(orderId);

            return Ok("Удалено из корзины");
        }

        private async Task UpdateOrderTotalPrice(int orderId)
        {
            var order = await _context.Orders.FirstOrDefaultAsync(o => o.Id == orderId);
            if (order != null)
            {
                order.TotalPrice = await _context.OrderItems
                    .Where(oi => oi.IdOrder == orderId)
                    .SumAsync(oi => (int)(oi.Quantity * oi.Price));

                await _context.SaveChangesAsync();
            }
        }
    }
}
