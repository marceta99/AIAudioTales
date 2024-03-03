using AIAudioTalesServer.Data.Interfaces;
using AIAudioTalesServer.Data.Repositories;
using AIAudioTalesServer.Models;
using AIAudioTalesServer.Models.DTOS;
using AIAudioTalesServer.Models.Enums;
using AutoMapper.Internal;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
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
            _frontendCanceledUrl = _appSettings.Value.ClientUrl + "/home/basket";
        }

        [HttpPost("PlaceOrder")]
        public async Task<ActionResult<string>> PlaceOrder([FromBody] BasketDTO basket)
        {
            try
            {   // Get the JWT token cookie
                var jwtTokenCookie = Request.Cookies["X-Access-Token"];

                if (!string.IsNullOrEmpty(jwtTokenCookie))
                {
                    // Decode the JWT token
                    var tokenHandler = new JwtSecurityTokenHandler();
                    var token = tokenHandler.ReadJwtToken(jwtTokenCookie);

                    // Access custom claim "email"
                    var emailClaim = token.Claims.FirstOrDefault(c => c.Type == "email");

                    if (emailClaim != null)
                    {
                        var email = emailClaim.Value;

                        var user = await _authRepository.GetUserWithEmail(email);
                        if (user == null) return BadRequest();

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
                    else
                    {
                        return BadRequest();
                    }
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception ex)
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
    }
}
