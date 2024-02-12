namespace AIAudioTalesServer.Models.DTOS
{
    public class BasketReturnDTO
    {
        public IList<BasketItemReturnDTO> BasketItems { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
