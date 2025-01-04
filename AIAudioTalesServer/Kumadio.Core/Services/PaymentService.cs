using AIAudioTalesServer.Core.Interfaces;
using AIAudioTalesServer.Domain.Entities;
using AIAudioTalesServer.Domain.Enums;
using AIAudioTalesServer.Infrastructure.Interfaces;
using AIAudioTalesServer.Settings;
using AIAudioTalesServer.Web.DTOS;
using Kumadio.Core.Interfaces;
using Kumadio.Domain.Entities;
using Kumadio.Domain.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Stripe;
using Stripe.Checkout;


namespace Kumadio.Core.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly AppSettings _appSettings;
        private readonly ILibraryService _libraryService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        private readonly string _frontendSuccessUrl;
        private readonly string _frontendCanceledUrl;

        public PaymentService(
            IPaymentRepository paymentRepository,
            IHttpContextAccessor httpContextAccessor,
            ILibraryService libraryService,
            IOptions<AppSettings> appSettings)
        {
            _paymentRepository = paymentRepository;
            _appSettings = appSettings.Value;
            _libraryService = libraryService;
            _httpContextAccessor = httpContextAccessor;
            _frontendSuccessUrl = _appSettings.ClientUrl + "/home/library";
            _frontendCanceledUrl = _appSettings.ClientUrl + "/home/basket#error";
        }

        public async Task<string?> PlaceOrderAsync(DTOBasket basket, User user)
        {
            // 1) Remove any pending purchases for that user (now via IBooksService)
            var result = await _libraryService.RemoveUserPendingPurchases(user);
            if (!result)
            {
                // Could throw an exception or just return null
                return null;
            }

            var basketItems = basket.Items;
            // 2) Initiate Stripe checkout

            var bookIds = new List<int>();

            foreach (var item in basketItems) {
                bookIds.Add(item.BookId);
            }

            var sessionId = await CheckOut(basketItems);

            // 3) Mark books as "purchase pending" (now via IBooksService)
            await _libraryService.PurchaseBooks(
                user.Id,
                bookIds,
                PurchaseType.BasicPurchase,
                Language.ENGLISH_USA,
                sessionId
            );

            // 4) Clear basket (now via IBooksService)
            await _libraryService.RemoveBasketItems(user.Id);

            return sessionId; // Return sessionId to the controller
        }

        public async Task<string> CheckOut(List<BasketItem> basketItems)
        {
            // Raw logic to create Stripe session
            var request = _httpContextAccessor.HttpContext.Request;
            var baseUrl = $"{request.Scheme}://{request.Host}";

            var lineItems = new List<SessionLineItemOptions>();

            foreach (var basketItem in basketItems)
            {
                lineItems.Add(new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = (long?)(basketItem.ItemPrice * 100),
                        Currency = "usd",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = basketItem.Book.Title,
                            Description = basketItem.Book.Description,
                            Images = new List<string> { basketItem.Book.ImageURL }
                        }
                    },
                    Quantity = 1
                });
            }

            var options = new SessionCreateOptions
            {
                SuccessUrl = $"{baseUrl}/api/payment/success?sessionId={{CHECKOUT_SESSION_ID}}",
                CancelUrl = $"{baseUrl}/api/payment/canceled?sessionId={{CHECKOUT_SESSION_ID}}",
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = lineItems,
                Mode = "payment",
                InvoiceCreation = new SessionInvoiceCreationOptions { Enabled = true }
            };

            var service = new SessionService();
            var session = await service.CreateAsync(options);

            return session.Id;
        }

        public async Task<string> GetSubscribeSessionId()
        {
            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = new List<SessionLineItemOptions>
                {
                    new SessionLineItemOptions
                    {
                        Price = "price_1OqHJLBQEYQBClyqiHgYJ2N9",
                        Quantity = 1,
                    },
                },
                Mode = "subscription",
                SuccessUrl = "http://localhost:4200/home/my-profile#success",
                CancelUrl = "http://localhost:4200/home/my-profile#error",
            };

            var service = new SessionService();
            var session = await service.CreateAsync(options);
            return session.Id;
        }

        public async Task<bool> CheckoutSuccessAsync(string sessionId)
        {
            // Mark purchase as success in DB (now via IBooksService)
            bool result = await _libraryService.UpdatePurchaseStatus(sessionId);
            return result;
        }

        public async Task<bool> CheckoutCanceledAsync(string sessionId)
        {
            // Remove canceled purchase (now via IBooksService)
            bool result = await _libraryService.RemoveCanceledPurchase(sessionId);
            return result;
        }

        public async Task ListenWebhookAsync(Stream requestBody, string stripeSignature)
        {
            // Example of replicating your Webhook logic from controller
            var endpointSecret = "whsec_8f61e7d5876e86d3...";
            var json = await new StreamReader(requestBody).ReadToEndAsync();

            try
            {
                var stripeEvent = EventUtility.ConstructEvent(json, stripeSignature, endpointSecret);

                switch (stripeEvent.Type)
                {
                    case Events.CustomerSubscriptionDeleted:
                        // Handle subscription canceled
                        break;

                    case Events.InvoicePaymentFailed:
                        // Payment failed
                        break;

                    case Events.InvoicePaymentSucceeded:
                        // Payment succeeded
                        break;

                    case Events.CustomerSubscriptionCreated:
                        // Subscription created
                        break;

                    case Events.CheckoutSessionCompleted:
                        var session = stripeEvent.Data.Object as Session;
                        if (session != null && session.Object == "checkout.session")
                        {
                            var sessionId = session.Id;
                            // Mark subscription as active in DB
                            await _paymentRepository.UpdateSubscriptionStatus(sessionId, SubscriptionStatus.Active);
                        }
                        break;

                    default:
                        break;
                }
            }
            catch (StripeException)
            {
                // Log or rethrow
                throw;
            }
        }

        public async Task<string?> CreateSubscribeSessionAsync(int userId)
        {
            // 1) Remove existing pending subscriptions for user
            await _paymentRepository.RemoveUserPendingSubscriptions(userId);

            // 2) Create new subscription session
            var sessionId = await GetSubscribeSessionId();
            if (sessionId == null) return null;

            // 3) Mark subscription as pending in DB
            await _paymentRepository.AddPendingSubscription(userId, sessionId);

            return sessionId;
        }
    }
}
