using AIAudioTalesServer.Models.DTOS;
using AIAudioTalesServer.Models.Enums;

namespace AIAudioTalesServer.Data.Interfaces
{
    public interface IPaymentRepository
    {
        Task<string> CheckOut(BasketDTO basket);
        Task<string> GetSubscribeSessionId();
        Task AddPendingSubscription(int userId, string sessionId);
        Task RemoveUserPendingSubscriptions(int userId);
        Task UpdateSubscriptionStatus(string sessionId, SubscriptionStatus status);
    }
}
