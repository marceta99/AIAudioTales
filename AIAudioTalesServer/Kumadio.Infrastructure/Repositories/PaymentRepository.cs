using Kumadio.Domain.Entities;
using Kumadio.Domain.Enums;
using Kumadio.Infrastructure.Data;
using Kumadio.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;


namespace Kumadio.Infrastructure.Repositories
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly AppDbContext _dbContext;
        private readonly IAuthRepository _authRepository;

        public PaymentRepository(
            AppDbContext dbContext,
            IAuthRepository authRepository)
        {
            _dbContext = dbContext;
            _authRepository = authRepository;
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
