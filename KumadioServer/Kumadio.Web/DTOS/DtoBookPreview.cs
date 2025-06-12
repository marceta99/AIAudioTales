namespace Kumadio.Web.DTOS
{
    public record DtoBookPreview
    (
        int Id,
        string Title,
        string Description,
        string ImageURL,
        int CategoryId,
        string RootPartAudio
    );
}
