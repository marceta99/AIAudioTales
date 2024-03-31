namespace AIAudioTalesServer.Models
{
    public class Company
    {
        public int Id { get; set; }
        public string CompanyName { get; set; }
        public IList<Job> Jobs { get; set; }



    }
}
