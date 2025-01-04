using AIAudioTalesServer.Web.DTOS;

namespace Kumadio.Core.Interfaces
{
    public interface IEditorService
    {
        Task<string?> UploadAsync(IFormFile file, HttpRequest request);
        Task<DTOReturnPart?> AddRootPartAsync(DTOCreateRootPart root, HttpRequest request);
        Task<DTOReturnPart?> AddBookPartAsync(DTOCreatePart part, HttpRequest request);
        Task<int> AddBookAsync(DTOCreateBook dto, int creatorId);
    }
}
