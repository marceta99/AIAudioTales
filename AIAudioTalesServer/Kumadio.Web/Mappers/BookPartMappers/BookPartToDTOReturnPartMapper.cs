using Kumadio.Domain.Entities;
using Kumadio.Web.DTOS;
using Kumadio.Web.Mappers.Base;

namespace Kumadio.Web.Mappers.BookPartMappers
{
    public class BookPartToDTOReturnPartMapper : BaseMapper<BookPart, DTOReturnPart>
    {
        private readonly IMapper<Answer, DTOReturnAnswer> _answerMapper;

        public BookPartToDTOReturnPartMapper(IMapper<Answer, DTOReturnAnswer> answerMapper)
        {
            _answerMapper = answerMapper;
        }
        public override DTOReturnPart MapCore(BookPart source)
        {
            return new DTOReturnPart
            {
                Id = source.Id,
                PartAudioLink = source.PartAudioLink,
                IsRoot = source.IsRoot,
                BookId = source.BookId,
                ParentAnwserId = source.ParentAnswer.Id,
                ParentAnswerText = source.ParentAnswer.Text,
                Answers = _answerMapper.Map(source.Answers)
            };
        }
    }
}
