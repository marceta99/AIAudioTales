using AIAudioTalesServer.Core.Interfaces;
using AIAudioTalesServer.Domain.Entities;
using AIAudioTalesServer.Domain.Enums;
using AIAudioTalesServer.Infrastructure.Interfaces;
using AIAudioTalesServer.Settings;
using AIAudioTalesServer.Web.DTOS;
using Microsoft.Extensions.Options;
using Stripe;
using Stripe.Checkout;

namespace AIAudioTalesServer.Core.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly AppSettings _appSettings;
        private readonly ILibraryService _libraryService;

        private readonly string _frontendSuccessUrl;
        private readonly string _frontendCanceledUrl;

        public PaymentService(
            IPaymentRepository paymentRepository,  
            ILibraryService libraryService,
            IOptions<AppSettings> appSettings)
        {
            _paymentRepository = paymentRepository;
            _appSettings = appSettings.Value;
            _libraryService = libraryService;

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

            // 2) Initiate Stripe checkout
            var sessionId = await _paymentRepository.CheckOut(basket);

            // 3) Mark books as "purchase pending" (now via IBooksService)
            await _libraryService.PurchaseBooks(
                user.Id,
                basket.BasketItems,
                PurchaseType.BasicPurchase,
                Language.ENGLISH_USA,
                sessionId
            );

            // 4) Clear basket (now via IBooksService)
            await _libraryService.RemoveBasketItems(user.Id);

            return sessionId; // Return sessionId to the controller
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
            var sessionId = await _paymentRepository.GetSubscribeSessionId();
            if (sessionId == null) return null;

            // 3) Mark subscription as pending in DB
            await _paymentRepository.AddPendingSubscription(userId, sessionId);

            return sessionId;
        }
    }
}
