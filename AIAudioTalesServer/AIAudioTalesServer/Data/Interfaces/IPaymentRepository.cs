using AIAudioTalesServer.Models.DTOS;

namespace AIAudioTalesServer.Data.Interfaces
{
    public interface IPaymentRepository
    {
        Task<string> CheckOut(BasketDTO basket);
    }
}
