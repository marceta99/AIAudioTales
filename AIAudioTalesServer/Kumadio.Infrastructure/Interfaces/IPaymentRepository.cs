

using Kumadio.Domain.Enums;

namespace Kumadio.Infrastructure.Interfaces
{
    public interface IPaymentRepository
    {
        Task AddPendingSubscription(int userId, string sessionId);
        Task RemoveUserPendingSubscriptions(int userId);
        Task UpdateSubscriptionStatus(string sessionId, SubscriptionStatus status);
    }
}
