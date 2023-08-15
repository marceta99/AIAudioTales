namespace AIAudioTalesServer.Models
{
    public class User
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public Role Role { get; set; }

        public IList<PurchasedBooks> PurchasedBooks { get; set; }

    }
}
