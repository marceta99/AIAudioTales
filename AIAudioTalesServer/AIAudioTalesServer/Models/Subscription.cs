using AIAudioTalesServer.Models.Enums;

namespace AIAudioTalesServer.Models
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
