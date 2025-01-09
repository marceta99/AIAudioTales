using Kumadio.Domain.Entities;
using Kumadio.Web.DTOS;
using Kumadio.Web.Mappers.Base;
using System.Diagnostics.CodeAnalysis;

namespace Kumadio.Web.Mappers.BookMappers
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
                BookId = source.BookId,
                IsRoot = source.IsRoot,
                ParentAnwserId = source.ParentAnswer.Id,
                ParentAnswerText = source.ParentAnswer.Text,
                Answers = source.Answers.Select(answer => _answerMapper.Map(answer)).ToList(),
            };
        }
    }
}
