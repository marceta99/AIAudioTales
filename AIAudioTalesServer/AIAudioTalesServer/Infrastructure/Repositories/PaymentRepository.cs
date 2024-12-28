using AIAudioTalesServer.Domain.Enums;
using AIAudioTalesServer.Infrastructure.Data;
using AIAudioTalesServer.Web.DTOS;
using Microsoft.EntityFrameworkCore;
using Stripe.Checkout;
using AIAudioTalesServer.Domain.Entities;
using AIAudioTalesServer.Infrastructure.Interfaces;

namespace AIAudioTalesServer.Infrastructure.Repositories
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly AppDbContext _dbContext;
        private readonly IAuthRepository _authRepository;

        public PaymentRepository(
            IHttpContextAccessor httpContextAccessor,
            AppDbContext dbContext,
            IAuthRepository authRepository)
        {
            _httpContextAccessor = httpContextAccessor;
            _dbContext = dbContext;
            _authRepository = authRepository;
        }

        public async Task<string> CheckOut(DTOBasket basket)
        {
            // Raw logic to create Stripe session
            var request = _httpContextAccessor.HttpContext.Request;
            var baseUrl = $"{request.Scheme}://{request.Host}";

            var lineItems = new List<SessionLineItemOptions>();

            foreach (var basketItem in basket.BasketItems)
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

        public async Task AddPendingSubscription(int userId, string sessionId)
        {
            var subscription = new Subscription
            {
                SessionId = sessionId,
                UserId = userId,
                SubscriptionStatus = SubscriptionStatus.Pending
            };

            await _dbContext.Subscriptions.AddAsync(subscription);
            await _dbContext.SaveChangesAsync();
        }

        public async Task RemoveUserPendingSubscriptions(int userId)
        {
            // Remove all pending subscriptions for user
            var pendingSubs = await _dbContext.Subscriptions
                .Where(s => s.UserId == userId && s.SubscriptionStatus == SubscriptionStatus.Pending)
                .ToListAsync();

            if (pendingSubs.Count > 0)
            {
                _dbContext.Subscriptions.RemoveRange(pendingSubs);
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task UpdateSubscriptionStatus(string sessionId, SubscriptionStatus status)
        {
            var subscription = await _dbContext.Subscriptions
                .FirstOrDefaultAsync(s => s.SessionId == sessionId);

            if (subscription == null) return;

            subscription.SubscriptionStatus = status;

            // If active, also update user role to "LISTENER_WITH_SUBSCRIPTION"
            if (status == SubscriptionStatus.Active)
            {
                await _authRepository.UpdateUserRole(Role.LISTENER_WITH_SUBSCRIPTION, subscription.UserId);
            }

            await _dbContext.SaveChangesAsync();
        }
    }
}
