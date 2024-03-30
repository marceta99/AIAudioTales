using AIAudioTalesServer.Models;

namespace AIAudioTalesServer.Data.Interfaces
{
    public interface IWorkerRepository
    {
        Task<IList<User>> GetAllUsers();
    }
}
