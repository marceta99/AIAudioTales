namespace AIAudioTalesServer.Models.DTOS
{
    public class DTOBasket
    {
        public IList<DTOReturnBasketItem> BasketItems { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
