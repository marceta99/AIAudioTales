using Kumadio.Domain.Entities;
using Kumadio.Web.DTOS;
using Kumadio.Web.Mappers.Base;

namespace Kumadio.Web.Mappers.BookMappers
{
    public class BookPartToDTOReturnPartMapper : DtoMapper<BookPart, DTOReturnPart>
    {
        private readonly IDtoMapper<Answer, DTOReturnAnswer> _answerMapper;

        public BookPartToDTOReturnPartMapper(IDtoMapper<Answer, DTOReturnAnswer> answerMapper)
        {
            _answerMapper = answerMapper;
        }
        public override DTOReturnPart MapCore(BookPart source)
        {
            return new DTOReturnPart
            {
                Id = source.Id,
                BookId = source.BookId,
                IsRoot = source.IsRoot,
                ParentAnwserId = source.ParentAnswer.Id,
                ParentAnswerText = source.ParentAnswer.Text,
                Answers = source.Answers.Select(answer => _answerMapper.Map(answer)).ToList(),
            };
        }
    }
}
