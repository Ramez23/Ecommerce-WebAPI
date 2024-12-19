using Microsoft.AspNetCore.Mvc;
using Stripe;
using Ecommerce.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Collections.Generic;
using Ecommerce.DTO;

namespace Ecommerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly string _stripeSecretKey = "sk_test_51PyOWlBN0RnOPILYrNnbRjrRs4JXdm3JuRibTSm7Gb7dHrlhrmvruKITxMIc2r1hpBTWKQLCEblkYOAicZ6eiGYm00T9GYEke5";

        public PaymentController(ApplicationDbContext context)
        {
            _context = context;
            StripeConfiguration.ApiKey = _stripeSecretKey;
        }

        [HttpPost("create-checkout-session")]
        public async Task<IActionResult> CreateCheckoutSession([FromBody] CreateCheckoutSessionDTO dto)
        {
            var order = await _context.Orders
                .Include(o => o.Products)
                .FirstOrDefaultAsync(o => o.Id == dto.OrderId);

            if (order == null)
            {
                return NotFound("Order not found.");
            }

            var totalPrice = order.Amount;

            var paymentIntentOptions = new PaymentIntentCreateOptions
            {
                Amount = (long)(totalPrice),
                Currency = "usd",
                PaymentMethodTypes = new List<string> { "card" },
                Metadata = new Dictionary<string, string>
        {
            { "OrderId", order.Id.ToString() },
            { "CustomerEmail", "customer@example.com" }
        }
            };

            var paymentIntentService = new PaymentIntentService();
            var paymentIntent = await paymentIntentService.CreateAsync(paymentIntentOptions);

            return Ok(new { clientSecret = paymentIntent.ClientSecret });
        }



        [HttpPost("payment-success")]
        public async Task<IActionResult> PaymentSuccess([FromBody] PaymentSuccessDTO paymentSuccessDTO)
        {
            var order = await _context.Orders
                .FirstOrDefaultAsync(o => o.Id == paymentSuccessDTO.OrderId);

            if (order == null)
            {
                return NotFound("Order not found.");
            }

            order.Method = PaymentMethods.creditCard; 
            await _context.SaveChangesAsync();

            var cart = await _context.Carts.FirstOrDefaultAsync(c => c.UserId == paymentSuccessDTO.UserId);
            if (cart != null)
            {
                var cartProducts = await _context.CartProducts.Where(cp => cp.CartId == cart.Id).ToListAsync();
                _context.CartProducts.RemoveRange(cartProducts);
                await _context.SaveChangesAsync();
            }

            return Ok(new { Message = "Payment Successful! Order confirmed." });
        }


        [HttpGet("list-payments")]
        public async Task<IActionResult> ListPayments()
        {
            try
            {
                var service = new PaymentIntentService();
                var options = new PaymentIntentListOptions
                {
                    Limit = 10,  
                };

                var paymentIntents = await service.ListAsync(options);

                if (paymentIntents.Data.Count == 0)
                {
                    return Ok(new { message = "No payments found." });
                }

                return Ok(paymentIntents);
            }
            catch (StripeException e)
            {
                return StatusCode(500, new { error = e.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}
