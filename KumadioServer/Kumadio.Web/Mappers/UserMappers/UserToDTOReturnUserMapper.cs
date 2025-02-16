using Kumadio.Domain.Entities;
using Kumadio.Web.DTOS;
using Kumadio.Web.Mappers.Base;

namespace Kumadio.Web.Mappers.UserMappers
{
    public class UserToDTOReturnUserMapper : DtoMapper<User, DTOReturnUser>
    {
        public override DTOReturnUser MapCore(User source)
        {
            return new DTOReturnUser
            {
                FirstName = source.FirstName,
                LastName = source.LastName,
                Email = source.Email,
                Role = source.Role
            };
        }
    }
}
