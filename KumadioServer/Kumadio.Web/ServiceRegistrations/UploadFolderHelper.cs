namespace Kumadio.Web.ServiceRegistrations
{
    public static class UploadFolderHelper
    {
        public static void CreateUploadFolder()
        {
            string uploadFolder = Path.Combine(Directory.GetCurrentDirectory(), "uploads");
            if (!Directory.Exists(uploadFolder))
            {
                Directory.CreateDirectory(uploadFolder);
            }
        }
    }
}
