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
        public IList<Book> CreatedBooks { get; set; }
        public IList<PurchasedBook> PurchasedBooks { get; set; }
        public RefreshToken RefreshToken { get; set; }
        public SearchHistory SearchHistory { get; set; }
        public Subscription Subscription { get; set; }
    }
}
