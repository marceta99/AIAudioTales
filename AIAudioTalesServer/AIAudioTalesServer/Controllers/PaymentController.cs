using AIAudioTalesServer.Data.Interfaces;
using AIAudioTalesServer.Models.DTOS;
using AutoMapper.Internal;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Stripe.Checkout;

namespace AIAudioTalesServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly IOptions<AppSettings> _appSettings;
        private readonly string _frontendSuccessUrl;
        private readonly string _frontendCanceledUrl;

        public PaymentController(IPaymentRepository paymentRepository, IOptions<AppSettings> appSettings)
        {
            this._paymentRepository = paymentRepository;
            this._appSettings = appSettings;
            _frontendSuccessUrl = _appSettings.Value.ClientUrl + "/home/success-purchase";
            _frontendCanceledUrl = _appSettings.Value.ClientUrl + "/home/cancel-purchase";
        }

        [HttpPost("PlaceOrder")]
        public async Task<ActionResult<string>> PlaceOrder([FromBody] BasketDTO basketDTO)
        {
            try
            {
                var sessionId = await _paymentRepository.CheckOut(basketDTO);
                return Ok(new { sessionId = sessionId});
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
        /// <summary>
        /// this API is going to be hit when order is placed successfully @Stripe
        /// </summary>
        /// <returns>A redirect to the front end success page</returns>
        [HttpGet("Success")]
        public async Task<ActionResult> CheckoutSuccess([FromQuery] string sessionId)
        {
            try
            {
                var sessionService = new SessionService();
                var session = sessionService.Get(sessionId);

                //var total = session.AmountTotal.Value; <- total from Stripe side also
                //var customerEmail = session.CustomerDetails.Email;

                //detalji koje mogu da izvucem o useru, neke ce da mi trebaju da cuvam u bazi najvise mejl da bih nasao Id usera
                //CustomerModel customer = new CustomerModel(session.Id, session.CustomerDetails.Name, session.CustomerDetails.Email, session.CustomerDetails.Phone);

                // _dbContext.savePurchase() moracu da cuvam u bazi da je user purchosavao knjigu

                return Redirect(_frontendSuccessUrl);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        /// <summary>
        /// this API is going to be hit when order is a failure
        /// </summary>
        /// <returns>A redirect to the front end success page</returns>
        [HttpGet("canceled")]
        public async Task<ActionResult> CheckoutCanceled()
        {
            try
            {

                // Insert here failure data in data base
                return Redirect(_frontendCanceledUrl);
            }
            catch (Exception ex)
            {
                return BadRequest("Problem with checkout");
            }


        }
    }
}
