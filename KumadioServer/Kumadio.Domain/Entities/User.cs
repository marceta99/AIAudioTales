using Kumadio.Domain.Enums;

namespace Kumadio.Domain.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
        public Role Role { get; set; }
        public ICollection<Book> CreatedBooks { get; set; }
        public ICollection<PurchasedBook> PurchasedBooks { get; set; }
        public RefreshToken RefreshToken { get; set; }
        public ICollection<SearchHistory> SearchHistories { get; set; }
        public Subscription Subscription { get; set; }
        public bool HasOnboarded { get; set; }
        public OnboardingData OnboardingData { get; set; }
    }
}
