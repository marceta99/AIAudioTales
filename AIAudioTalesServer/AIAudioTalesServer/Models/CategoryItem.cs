namespace AIAudioTalesServer.Models
{
    public class CategoryItem
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public string ItemName { get; set; }

        public int CategoryId { get; set; }
        public Category Category { get; set; }

    }
}
