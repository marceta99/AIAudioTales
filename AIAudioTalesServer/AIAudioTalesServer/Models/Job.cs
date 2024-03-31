namespace AIAudioTalesServer.Models
{
    public class Job
    {
        public int Id { get; set; }
        public string JobName { get; set; }
        public int CompanyId { get; set; }
        public Company Company { get; set; }
        public IList<Category> Categories { get; set; }
        public IList<User> Workers { get; set; }


    }
}
