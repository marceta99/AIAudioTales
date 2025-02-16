using Kumadio.Domain.Enums;

namespace Kumadio.Domain.Entities
{
    public class Subscription
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public string SessionId { get; set; }
        public SubscriptionStatus SubscriptionStatus { get; set; }
    }
}
