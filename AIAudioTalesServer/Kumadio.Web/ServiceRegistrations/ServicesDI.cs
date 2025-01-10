using Kumadio.Core.Interfaces;
using Kumadio.Core.Services;

namespace Kumadio.Web.ServiceRegistrations
{
    public static class ServicesDI
    {
        public static IServiceCollection AddKumadioServices(this IServiceCollection services)
        {
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IPaymentService, PaymentService>();
            services.AddScoped<ICatalogService, CatalogService>();
            services.AddScoped<IEditorService, EditorService>();
            services.AddScoped<ILibraryService, LibraryService>();

            return services;
        }
    }
}
