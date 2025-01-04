namespace Kumadio.Web.DTOS
{
    public class DTOReturnBasketItem
    {
        public int Id { get; set; }
        public decimal ItemPrice { get; set; }
        public int BookId { get; set; }
        public DTOReturnBook Book { get; set; }
    }
}
