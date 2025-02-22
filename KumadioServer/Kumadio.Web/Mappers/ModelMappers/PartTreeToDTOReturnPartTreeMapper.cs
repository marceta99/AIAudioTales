using Kumadio.Core.Models;
using Kumadio.Domain.Entities;
using Kumadio.Web.DTOS;
using Kumadio.Web.Mappers.Base;

namespace Kumadio.Web.Mappers.ModelMappers
{
    public class PartTreeToDTOReturnPartTreeMapper : DtoMapper<PartTreeModel, DTOReturnPartTree>
    {
        private readonly IDtoMapper<Answer, DTOReturnAnswer> _answerMapper;

        public PartTreeToDTOReturnPartTreeMapper(IDtoMapper<Answer, DTOReturnAnswer> answerMapper)
        {
            _answerMapper = answerMapper;
        }
        public override DTOReturnPartTree MapCore(PartTreeModel source)
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
