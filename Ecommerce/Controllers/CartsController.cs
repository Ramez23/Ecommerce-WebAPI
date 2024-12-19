using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ecommerce.Models;
using Ecommerce.DTO;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Ecommerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CartController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddProductToCart([FromBody] AddToCartDTO addToCartDTO)
        {
            if (addToCartDTO == null)
            {
                return BadRequest("Invalid data.");
            }

            var cart = await _context.Carts
                .Include(c => c.CartProducts)
                .FirstOrDefaultAsync(c => c.UserId == addToCartDTO.UserId);

            if (cart == null)
            {
                cart = new Cart
                {
                    UserId = addToCartDTO.UserId,
                    CartProducts = new List<CartProduct>()
                };
                await _context.Carts.AddAsync(cart);
                await _context.SaveChangesAsync();
            }

            var cartProduct = await _context.CartProducts
                .FirstOrDefaultAsync(cp => cp.CartId == cart.Id && cp.ProductId == addToCartDTO.ProductId);

            if (cartProduct == null)
            {
                cartProduct = new CartProduct
                {
                    CartId = cart.Id,
                    ProductId = addToCartDTO.ProductId,
                    Quantity = addToCartDTO.Quantity
                };
                await _context.CartProducts.AddAsync(cartProduct);
            }
            else
            {
                cartProduct.Quantity += addToCartDTO.Quantity;
            }

            await _context.SaveChangesAsync();
            return Ok(new { message = "Product added to cart successfully" });
        }

        [HttpGet("get/{userId}")]
        public async Task<IActionResult> GetCartByUserId(string userId)
        {
            var cart = await _context.Carts
                .Include(c => c.CartProducts)
                .ThenInclude(cp => cp.Product)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null)
            {
                return NotFound(new { message = "Cart not found for the given UserId." });
            }

            var productsToRemove = cart.CartProducts.Where(cp => cp.Quantity < 1).ToList();
            if (productsToRemove.Any())
            {
                foreach (var productToRemove in productsToRemove)
                {
                    _context.CartProducts.Remove(productToRemove);
                }
                await _context.SaveChangesAsync();
            }

            var cartDTO = new
            {
                CartId = cart.Id,
                Products = cart.CartProducts.Select(cp => new
                {
                    cp.ProductId,
                    ProductName = cp.Product.Name,
                    cp.Product.Price,
                    cp.Quantity,
                    TotalPrice = cp.Product.Price * cp.Quantity
                }).ToList()
            };

            return Ok(cartDTO);
        }

        [HttpDelete("remove/{cartProductId}")]
        public async Task<IActionResult> RemoveProductFromCart(int cartProductId)
        {
            var cartProduct = await _context.CartProducts
                .FirstOrDefaultAsync(cp => cp.Id == cartProductId);

            if (cartProduct == null)
            {
                return NotFound(new { message = "Product not found in the cart." });
            }

            _context.CartProducts.Remove(cartProduct);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Product removed from the cart successfully" });
        }
    }
}
