namespace Kumadio.Web.Settings
{
    public class AppSettings
    {
        public string Secret { get; set; }
        public string GoogleClientId { get; set; }
        public string StripeSecretKey { get; set; }
        public string[] ClientUrls { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
    }

}
