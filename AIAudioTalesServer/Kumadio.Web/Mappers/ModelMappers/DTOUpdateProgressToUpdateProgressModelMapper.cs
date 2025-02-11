using Kumadio.Core.Models;
using Kumadio.Web.DTOS;
using Kumadio.Web.Mappers.Base;

namespace Kumadio.Web.Mappers.ModelMappers
{
    public class DTOUpdateProgressToUpdateProgressModelMapper : DtoMapper<DTOUpdateProgress, UpdateProgressModel>
    {
        public override UpdateProgressModel MapCore(DTOUpdateProgress source)
        {
            return new UpdateProgressModel
            {
                BookId = source.BookId,
                NextBookId = source.NextBookId,
                PlayingPosition = source.PlayingPosition,
                QuestionsActive = source.QuestionsActive
            };
        }
    }
}
