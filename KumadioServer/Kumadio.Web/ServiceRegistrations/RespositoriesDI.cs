using Kumadio.Core.Common.Interfaces;
using Kumadio.Core.Common.Interfaces.Base;
using Kumadio.Infrastructure.DiskFileStorage;
using Kumadio.Infrastructure.Repositories.Base;
using Kumadio.Infrastructure.Repositories.Domain;

namespace Kumadio.Web.ServiceRegistrations
{
    public static class RespositoriesDI
    {
        public static IServiceCollection AddKumadioRepositories(this IServiceCollection services)
        {
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IAnswerRepository, AnswerRepository>();
            services.AddScoped<IBookPartRepository, BookPartRepository>();
            services.AddScoped<IBookRepository, BookRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<IPurchasedBookRepository, PurchasedBookRepository>();
            services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
            services.AddScoped<ISearchHistoryRepository, SearchHistoryRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IOnboardingDataRepository, OnboardingDataRepository>();
            services.AddScoped<IOnboardingQuestionRepository, OnboardingQuestionRepository>();
            services.AddScoped<ISelectedOptionRepository, SelectedOptionRepository>();

            services.AddScoped<IFileStorage, DiskFileStorageService>();

            return services;
        }
    }
}
