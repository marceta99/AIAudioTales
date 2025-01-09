using Kumadio.Domain.Entities;
using Kumadio.Domain.Enums;
using Kumadio.Web.DTOS.Auth;
using Kumadio.Web.Mappers.Base;

namespace Kumadio.Web.Mappers.UserMappers
{
    public class DTORegisterCreatorToUserMapper : BaseMapper<DTORegisterCreator, User>
    {
        public override User MapCore(DTORegisterCreator source)
        {
            return new User
            {
                FirstName = source.FirstName,
                LastName = source.LastName,
                Email = source.Email,
                Role = Role.CREATOR
            };
        }
    }
}
