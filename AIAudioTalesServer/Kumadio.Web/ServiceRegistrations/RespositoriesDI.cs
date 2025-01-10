using Kumadio.Infrastructure.Interfaces;
using Kumadio.Infrastructure.Repositories;

namespace Kumadio.Web.ServiceRegistrations
{
    public static class RespositoriesDI
    {
        public static IServiceCollection AddKumadioRepositories(this IServiceCollection services)
        {
            services.AddScoped<IAuthRepository, AuthRepository>();
            services.AddScoped<IPaymentRepository, PaymentRepository>();
            services.AddScoped<ICatalogRepository, CatalogRepository>();
            services.AddScoped<IEditorRepository, EditorRepository>();
            services.AddScoped<ILibraryRepository, LibraryRepository>();

            return services;
        }
    }
}
