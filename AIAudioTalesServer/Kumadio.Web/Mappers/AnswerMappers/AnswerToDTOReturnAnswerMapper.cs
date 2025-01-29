using Kumadio.Domain.Entities;
using Kumadio.Web.DTOS;
using Kumadio.Web.Mappers.Base;

namespace Kumadio.Web.Mappers.AnswerMappers
{
    public class AnswerToDTOReturnAnswerMapper : BaseMapper<Answer, DTOReturnAnswer>
    {
        public override DTOReturnAnswer MapCore(Answer source)
        {
            return new DTOReturnAnswer 
            {
                Id = source.Id,
                CurrentPartId = source.CurrentPartId,
                NextPartId = source.NextPartId,
                Text = source.Text
            };
        }
    }
}
