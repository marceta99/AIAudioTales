namespace AIAudioTalesServer.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string CategoryName { get; set; }
        public int JobId { get; set; }
        public Job Job { get; set; }

        public IList<CategoryItem> CategoryItems { get; set; }

    }
}
