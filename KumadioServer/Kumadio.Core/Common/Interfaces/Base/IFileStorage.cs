namespace Kumadio.Core.Common.Interfaces.Base
{
    public interface IFileStorage
    {
        Task<string?> SaveFile(byte[] fileBytes, string extension, string baseUrl);
        Task<bool> DeleteFile(string fileLink);
    }
}
