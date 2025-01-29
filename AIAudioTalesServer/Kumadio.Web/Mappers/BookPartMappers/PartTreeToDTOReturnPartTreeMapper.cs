using Kumadio.Core.Models;
using Kumadio.Domain.Entities;
using Kumadio.Web.DTOS;
using Kumadio.Web.Mappers.Base;

namespace Kumadio.Web.Mappers.BookPartMappers
{
    public class PartTreeToDTOReturnPartTreeMapper : BaseMapper<PartTree, DTOReturnPartTree>
    {
        private readonly IMapper<Answer, DTOReturnAnswer> _answerMapper;

        public PartTreeToDTOReturnPartTreeMapper(IMapper<Answer, DTOReturnAnswer> answerMapper)
        {
            _answerMapper = answerMapper;
        }
        public override DTOReturnPartTree MapCore(PartTree source)
        {
            return new DTOReturnPartTree
            {
                PartId = source.PartId,
                PartName = source.PartName,
                Answers = _answerMapper.Map(source.Answers)!
            };
        }
    }
}
