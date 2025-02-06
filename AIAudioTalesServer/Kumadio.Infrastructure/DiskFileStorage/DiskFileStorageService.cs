using Kumadio.Core.Common.Interfaces.Base;

namespace Kumadio.Infrastructure.DiskFileStorage
{
    public class DiskFileStorageService : IFileStorage
    {
        private readonly string _uploadFolder = Path.Combine(Directory.GetCurrentDirectory(), "uploads");

        public async Task<string?> SaveFile(byte[] fileBytes, string extension, string host)
        {
            if (fileBytes == null || fileBytes.Length == 0)
            {
                return null;
            }

            var fileName = Guid.NewGuid().ToString("N") + extension;
            var filePath = Path.Combine(_uploadFolder, fileName);

            try
            {
                await File.WriteAllBytesAsync(filePath, fileBytes);
            }
            catch
            {
                return null;
            }

            var accessLink = $"https://{host}/uploads/{fileName}";
            return accessLink;
        }

        public async Task<bool> DeleteFile(string fileLink)
        {
            if (string.IsNullOrEmpty(fileLink))
                return false;

            try
            {
                // Assuming fileLink is in the format: https://{host}/uploads/{fileName}
                var uri = new Uri(fileLink);
                var fileName = Path.GetFileName(uri.LocalPath);
                var filePath = Path.Combine(_uploadFolder, fileName);

                if (File.Exists(filePath))
                {
                    // The File.Delete method provided by .NET is synchronous. This means it blocks the calling thread until the deletion is complete,
                    // by wrapping File.Delete inside a Task.Run,that offload the work to a thread pool thread. This way, that await the deletion without blocking the main thread.
                    await Task.Run(() => File.Delete(filePath));
                }

                return true;
            }
            catch
            {
                // Optionally log the error here.
                return false;
            }
        }
    }
}
