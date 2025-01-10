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
            services.AddScoped<IMapper<Answer, DTOReturnAnswer>, AnswerToDTOReturnAnswerMapper>();
            services.AddScoped<IMapper<BasketItem, DTOReturnBasketItem>, BasketItemToDTOReturnBasketItemMapper>();
            services.AddScoped<IMapper<BookPart, DTOReturnPart>, BookPartToDTOReturnPartMapper>();
            services.AddScoped<IMapper<Book, DTOReturnBook>, BookToDTOReturnBookMapper>();
            services.AddScoped<IMapper<DTOCreateBook, Book>, DTOCreateBookToBookMapper>();
            services.AddScoped<IMapper<DTORegisterCreator, User>, DTORegisterCreatorToUserMapper>();
            services.AddScoped<IMapper<DTORegister, User>, DTORegisterToUserMapper>();
            services.AddScoped<IMapper<User, DTOReturnUser>, UserToDTOReturnUserMapper>();

            return services;
        }
    }
}
