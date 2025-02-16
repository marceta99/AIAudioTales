using Kumadio.Core.Interfaces;
using Kumadio.Core.Services;
using Kumadio.Domain.Entities;
using Kumadio.Web.DTOS.Auth;
using Kumadio.Web.DTOS;
using Kumadio.Web.Mappers.Base;
using Kumadio.Web.Mappers.BookMappers;
using Kumadio.Web.Mappers.UserMappers;

namespace Kumadio.Web.ServiceRegistrations
{
    public static class MappersDI
    {
        public static IServiceCollection AddKumadioMappers(this IServiceCollection services)
        {
            services.AddScoped<IDtoMapper<Answer, DTOReturnAnswer>, AnswerToDTOReturnAnswerMapper>();
            services.AddScoped<IDtoMapper<BookPart, DTOReturnPart>, BookPartToDTOReturnPartMapper>();
            services.AddScoped<IDtoMapper<Book, DTOReturnBook>, BookToDTOReturnBookMapper>();
            services.AddScoped<IDtoMapper<DTORegisterCreator, User>, DTORegisterCreatorToUserMapper>();
            services.AddScoped<IDtoMapper<DTORegister, User>, DTORegisterToUserMapper>();
            services.AddScoped<IDtoMapper<User, DTOReturnUser>, UserToDTOReturnUserMapper>();

            return services;
        }
    }
}
