using AIAudioTalesServer.Domain.Enums;
using AIAudioTalesServer.Web.DTOS;

namespace AIAudioTalesServer.Infrastructure.Interfaces
{
    public interface IPaymentRepository
    {
        Task<string> CheckOut(DTOBasket basket);
        Task<string> GetSubscribeSessionId();
        Task AddPendingSubscription(int userId, string sessionId);
        Task RemoveUserPendingSubscriptions(int userId);
        Task UpdateSubscriptionStatus(string sessionId, SubscriptionStatus status);
    }
}
