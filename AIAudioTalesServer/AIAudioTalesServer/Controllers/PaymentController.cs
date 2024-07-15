using AIAudioTalesServer.Data.Interfaces;
using AIAudioTalesServer.Data.Repositories;
using AIAudioTalesServer.Models;
using AIAudioTalesServer.Models.DTOS;
using AIAudioTalesServer.Models.Enums;
using AutoMapper.Internal;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Stripe;
using Stripe.Checkout;
using System.IdentityModel.Tokens.Jwt;
using System.Net;

namespace AIAudioTalesServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly IOptions<AppSettings> _appSettings;
        private readonly IBooksRepository _booksRepository;
        private readonly IAuthRepository _authRepository;
        private readonly string _frontendSuccessUrl;
        private readonly string _frontendCanceledUrl;

        public PaymentController(
            IPaymentRepository paymentRepository,
            IOptions<AppSettings> appSettings,
            IBooksRepository booksRepository,
            IAuthRepository authRepository
            )
        {
            this._paymentRepository = paymentRepository;
            this._appSettings = appSettings;
            this._booksRepository = booksRepository;
            this._authRepository = authRepository;
            _frontendSuccessUrl = _appSettings.Value.ClientUrl + "/home/library";
            _frontendCanceledUrl = _appSettings.Value.ClientUrl + "/home/basket#error";
        }

        [HttpPost("PlaceOrder")]
        public async Task<IActionResult> PlaceOrder([FromBody] DTOBasket basket)
        {
            try
            {
                var user = HttpContext.Items["CurrentUser"] as User;
                if (user == null)
                {
                    return Unauthorized();
                }

                var result = await _booksRepository.RemoveUserPendingPurchases(user);

                if (result)
                {
                    var sessionId = await _paymentRepository.CheckOut(basket);

                    await _booksRepository.PurchaseBooks(user.Id, basket.BasketItems, PurchaseType.BasicPurchase, Language.ENGLISH_USA, sessionId);

                    await _booksRepository.RemoveBasketItems(user.Id);

                    return Ok(new { sessionId = sessionId });
                }

                return BadRequest();
            }
            catch (Exception ex)
            {
                return BadRequest("There was a problem");
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
                var result = await _booksRepository.UpdatePurchaseStatus(sessionId);
                
                if (result)
                {
                    return Redirect(_frontendSuccessUrl);
                }
                return BadRequest();

                /*I dont need this session info righ need but I leave it here if I need it later somethimes
                //var sessionService = new SessionService();
                //var session = sessionService.Get(sessionId);

                //var total = session.AmountTotal.Value; <- total from Stripe side also
                //var customerEmail = session.CustomerDetails.Email;

                //detalji koje mogu da izvucem o useru, neke ce da mi trebaju da cuvam u bazi najvise mejl da bih nasao Id usera
                //CustomerModel customer = new CustomerModel(session.Id, session.CustomerDetails.Name, session.CustomerDetails.Email, session.CustomerDetails.Phone);*/

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
        public async Task<ActionResult> CheckoutCanceled([FromQuery] string sessionId)
        {
            try
            {
                var result = await _booksRepository.RemoveCanceledPurchase(sessionId);
                if (result)
                {
                    return Redirect(_frontendCanceledUrl);
                }
                return BadRequest();
            }
            catch (Exception ex)
            {
                return BadRequest("Problem with checkout");
            }


        }

        [HttpPost("ListenWebHook")]
        public async Task<IActionResult> ListenWebhook()
        {
            var endpointSecret = "whsec_8f61e7d5876e86d3d84907f39baad3ee309ffd4b7a7770bfdc46ff8da28756f1";
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
            try
            {
                var stripeEvent = EventUtility.ConstructEvent(json,
                    Request.Headers["Stripe-Signature"], endpointSecret);

                // Handle the event
                switch (stripeEvent.Type)
                {
                    case Events.CustomerSubscriptionDeleted:
                            break;
                    case Events.InvoicePaymentFailed:
                            break;
                    case Events.InvoicePaymentSucceeded:
                            break;
                    case Events.CustomerSubscriptionCreated:
                            break;
                    case Events.CheckoutSessionCompleted:
                        var session = stripeEvent.Data.Object as Session;
                        //if (session != null && session.Object == "subscription")
                        //{
                            // Retrieve the session ID from the event
                            var sessionId = session.Id;

                            // Update the subscription status to "active" and role to LISTENER_WITH_SUBSCRIPTION
                            await _paymentRepository.UpdateSubscriptionStatus(sessionId, SubscriptionStatus.Active);    
                        //}
                        break;
                    default:
                        break;
                }

                return Ok();
            }
            catch (StripeException e)
            {
                return BadRequest();
            }
        }

        [HttpPost("CreateSubscribeSession")]
        public async Task<IActionResult> CreateSubscribeSession()
        {
            try
            {
                var user = HttpContext.Items["CurrentUser"] as User;
                if (user == null)
                {
                    return Unauthorized();
                }
                //remove users previous pending subscriptions
                await _paymentRepository.RemoveUserPendingSubscriptions(user.Id);

                var sessionId = await _paymentRepository.GetSubscribeSessionId();

                await _paymentRepository.AddPendingSubscription(user.Id, sessionId);

                return Ok(new { sessionId = sessionId });    
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }
    }
}
