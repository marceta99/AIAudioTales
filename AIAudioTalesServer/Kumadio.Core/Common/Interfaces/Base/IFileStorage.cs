namespace Kumadio.Core.Common.Interfaces.Base
{
    public interface IFileStorage
    {
        Task<string?> SaveFile(byte[] fileBytes, string extension, string host);
        Task<bool> DeleteFile(string fileLink);
    }
}
