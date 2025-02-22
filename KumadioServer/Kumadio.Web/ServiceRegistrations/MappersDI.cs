using Kumadio.Domain.Entities;
using Kumadio.Web.DTOS.Auth;
using Kumadio.Web.DTOS;
using Kumadio.Web.Mappers.Base;
using Kumadio.Web.Mappers.BookMappers;
using Kumadio.Web.Mappers.UserMappers;
using Kumadio.Core.Models;
using Kumadio.Web.Mappers.BookPartMappers;
using Kumadio.Web.Mappers.AnswerMappers;
using Kumadio.Web.Mappers.CategoryMappers;
using Kumadio.Web.Mappers.ModelMappers;
using Kumadio.Web.Mappers.PurchasedBookMappers;

namespace Kumadio.Web.ServiceRegistrations
{
    public static class MappersDI
    {
        public static IServiceCollection AddKumadioMappers(this IServiceCollection services)
        {
            // Domain mappers
            services.AddScoped<IDtoMapper<Answer, DTOReturnAnswer>, AnswerToDTOReturnAnswerMapper>();
            services.AddScoped<IDtoMapper<BookPart, DTOReturnPart>, BookPartToDTOReturnPartMapper>();
            services.AddScoped<IDtoMapper<Book, DTOReturnBook>, BookToDTOReturnBookMapper>();
            services.AddScoped<IDtoMapper<DTORegisterCreator, User>, DTORegisterCreatorToUserMapper>();
            services.AddScoped<IDtoMapper<DTORegister, User>, DTORegisterToUserMapper>();
            services.AddScoped<IDtoMapper<User, DTOReturnUser>, UserToDTOReturnUserMapper>();
            services.AddScoped<IDtoMapper<Category, DTOReturnCategory>, CategoryToDTOReturnCategoryMapper>();
            services.AddScoped<IDtoMapper<PurchasedBook, DTOReturnPurchasedBook>, PurchasedBookToDTOReturnPurchasedBookMapper>();

            // Model mappers
            services.AddScoped<IDtoMapper<PartTreeModel, DTOReturnPartTree>, PartTreeToDTOReturnPartTreeMapper>();
            services.AddScoped<IDtoMapper<DTOUpdateProgress, UpdateProgressModel>, DTOUpdateProgressToUpdateProgressModelMapper>();


            return services;
        }
    }
}
