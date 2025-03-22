using Kumadio.Domain.Entities;
using Kumadio.Web.DTOS;
using Kumadio.Web.Mappers.Base;

namespace Kumadio.Web.Mappers.BookPartMappers
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
                PartAudioLink = source.PartAudioLink,
                IsRoot = source.IsRoot,
                BookId = source.BookId,
                ParentAnswer = _answerMapper.Map(source.ParentAnswer),
                Answers = _answerMapper.Map(source.Answers)
            };
        }
    }
}
