namespace AIAudioTalesServer.Models.DTOS
{
    public class BasketDTO
    {
        public IList<BasketItemReturnDTO> BasketItems { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
