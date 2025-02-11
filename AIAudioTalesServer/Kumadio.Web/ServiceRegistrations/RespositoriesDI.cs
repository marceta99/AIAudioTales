using Kumadio.Core.Common.Interfaces;
using Kumadio.Infrastructure.Interfaces;
using Kumadio.Infrastructure.Repositories;
using Kumadio.Infrastructure.Repositories.Domain;

namespace Kumadio.Web.ServiceRegistrations
{
    public static class RespositoriesDI
    {
        public static IServiceCollection AddKumadioRepositories(this IServiceCollection services)
        {
            services.AddScoped<IAnswerRepository, AnswerRepository>();
            services.AddScoped<IBookPartRepository, BookPartRepository>();
            services.AddScoped<IBookRepository, BookRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<IPurchasedBookRepository, PurchasedBookRepository>();
            services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
            services.AddScoped<ISearchHistoryRepository, SearchHistoryRepository>();
            services.AddScoped<IUserRepository, UserRepository>();

            return services;
        }
    }
}
