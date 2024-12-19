using Microsoft.AspNetCore.Mvc;
using Ecommerce.Models;
using Ecommerce.DTO;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace Ecommerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public OrderController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderDTO createOrderDTO)
        {
            if (createOrderDTO == null || createOrderDTO.CartId <= 0)
            {
                return BadRequest("Invalid request.");
            }
            var cart = await _context.Carts
                .Include(c => c.CartProducts)
                .ThenInclude(cp => cp.Product)
                .FirstOrDefaultAsync(c => c.Id == createOrderDTO.CartId);

            if (cart == null || !cart.CartProducts.Any())
            {
                return NotFound("Cart not found or no items in the cart.");
            }

            decimal totalAmount = cart.CartProducts.Sum(cp => cp.Quantity * cp.Product.Price);

            var order = new Order
            {
                UserId = cart.UserId,
                Amount = totalAmount,
                Date = DateTime.UtcNow,
                Method = PaymentMethods.creditCard, 
            };

            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();

            foreach (var cartProduct in cart.CartProducts)
            {
                var orderDetail = new OrderDetail
                {
                    ProductId = cartProduct.ProductId,
                    OrderId = order.Id,
                    Price = cartProduct.Product.Price,
                    Quantity = cartProduct.Quantity
                };

                _context.OrderDetails.Add(orderDetail);
            }

            await _context.SaveChangesAsync();

            return Ok(new { orderId = order.Id });
        }

    }
}
