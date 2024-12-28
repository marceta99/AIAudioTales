
using AIAudioTalesServer.Core.Interfaces;
using AIAudioTalesServer.Domain.Entities;
using AIAudioTalesServer.Domain.Enums;
using AIAudioTalesServer.Infrastructure.Interfaces;
using AIAudioTalesServer.Settings;
using AIAudioTalesServer.Web.DTOS;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Stripe;
using Stripe.Checkout;

namespace AIAudioTalesServer.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpPost("PlaceOrder")]
        public async Task<IActionResult> PlaceOrder([FromBody] DTOBasket basket)
        {
            // The current user is typically set by your UserContextMiddleware or similar
            var user = HttpContext.Items["CurrentUser"] as User;
            if (user == null)
                return Unauthorized();

            var sessionId = await _paymentService.PlaceOrderAsync(basket, user);

            if (string.IsNullOrEmpty(sessionId))
                return BadRequest("Failed to place order.");

            return Ok(new { sessionId });
        }

        [HttpGet("Success")]
        public async Task<IActionResult> CheckoutSuccess([FromQuery] string sessionId)
        {
            bool updated = await _paymentService.CheckoutSuccessAsync(sessionId);
            if (!updated) return BadRequest("Failed to update purchase status.");

            // Optionally redirect to the front-end success page 
            return RedirectPermanent("~/home/library");
            // or return Ok("Purchase successful") or however you prefer
        }

        [HttpGet("canceled")]
        public async Task<IActionResult> CheckoutCanceled([FromQuery] string sessionId)
        {
            bool removed = await _paymentService.CheckoutCanceledAsync(sessionId);
            if (!removed) return BadRequest("Failed to remove canceled purchase.");

            // Optionally redirect to the front-end canceled page
            return RedirectPermanent("~/home/basket#error");
        }

        [HttpPost("ListenWebHook")]
        public async Task<IActionResult> ListenWebhook()
        {
            var stripeSignature = Request.Headers["Stripe-Signature"];
            try
            {
                await _paymentService.ListenWebhookAsync(Request.Body, stripeSignature);
                return Ok();
            }
            catch (Exception ex)
            {
                // Log or handle error
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("CreateSubscribeSession")]
        public async Task<IActionResult> CreateSubscribeSession()
        {
            var user = HttpContext.Items["CurrentUser"] as User;
            if (user == null)
                return Unauthorized();

            var sessionId = await _paymentService.CreateSubscribeSessionAsync(user.Id);
            if (string.IsNullOrEmpty(sessionId))
                return BadRequest("Failed to create subscription session.");

            return Ok(new { sessionId });
        }
    }
}
