using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ecommerce.Models;
using Ecommerce.DTO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace Ecommerce.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CartProductsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CartProductsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("{cartId}")]
        public async Task<ActionResult<IEnumerable<CartProductDTO>>> GetCartProducts(int cartId)
        {
            var cartProducts = await _context.CartProducts
                .Where(cp => cp.CartId == cartId)
                .Include(cp => cp.Product)
                .Select(cp => new CartProductDTO
                {
                    ProductId = cp.ProductId,
                    ProductName = cp.Product.Name,
                    Price = cp.Product.Price,
                    Quantity = cp.Quantity
                })
                .ToListAsync();

            if (cartProducts == null || cartProducts.Count == 0)
            {
                return NotFound("No products found in this cart.");
            }

            return Ok(cartProducts);
        }

        [HttpPut("{cartId}/{productId}")]
        public async Task<IActionResult> UpdateCartProductQuantity(int cartId, int productId, [FromBody] UpdateCartProductDTO updateCartProductDTO)
        {
            var cartProduct = await _context.CartProducts
                .FirstOrDefaultAsync(cp => cp.CartId == cartId && cp.ProductId == productId);

            if (cartProduct == null)
            {
                return NotFound("Product not found in the cart.");
            }

            cartProduct.Quantity = updateCartProductDTO.Quantity;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while updating the product quantity: " + ex.Message);
            }

            return NoContent();
        }

        [HttpDelete("{cartId}/{productId}")]
        public async Task<IActionResult> RemoveCartProduct(int cartId, int productId)
        {
            var cartProduct = await _context.CartProducts
                .FirstOrDefaultAsync(cp => cp.CartId == cartId && cp.ProductId == productId);

            if (cartProduct == null)
            {
                return NotFound("Product not found in the cart.");
            }

            _context.CartProducts.Remove(cartProduct);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet("total/{cartId}")]
        public async Task<ActionResult<decimal>> GetCartTotalPrice(int cartId)
        {
            var cartProducts = await _context.CartProducts
                .Where(cp => cp.CartId == cartId)
                .Include(cp => cp.Product)
                .ToListAsync();

            if (cartProducts == null || cartProducts.Count == 0)
            {
                return NotFound("No products found in this cart.");
            }

            decimal totalPrice = cartProducts.Sum(cp => cp.Product.Price * cp.Quantity);

            return Ok(new { TotalPrice = totalPrice });
        }
        private bool CartProductExists(int cartId, int productId)
        {
            return _context.CartProducts.Any(e => e.CartId == cartId && e.ProductId == productId);
        }
    }
}
