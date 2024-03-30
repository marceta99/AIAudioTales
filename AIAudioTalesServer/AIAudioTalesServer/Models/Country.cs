namespace AIAudioTalesServer.Models
{
    public class Country
    {
        public int Id { get; set; }
        public string CountryName { get; set; }

        public IList<User> Citizens { get; set; }

    }
}
