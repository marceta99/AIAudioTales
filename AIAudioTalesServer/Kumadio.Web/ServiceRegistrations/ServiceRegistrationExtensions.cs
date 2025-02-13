using Kumadio.Infrastructure.Data;
using Kumadio.Web.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Kumadio.Web.ServiceRegistrations
{
    public static class ServiceRegistrationExtensions
    {
        public static IServiceCollection AddCustomServices(this IServiceCollection services, ConfigurationManager configuration)
        {
            // 1) Form Options
            services.Configure<FormOptions>(options =>
             {
                 options.MultipartBodyLengthLimit = 104857600; // Example limit of 100MB
             });

            // 2) DB Context
            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
            });
            services.AddDatabaseDeveloperPageExceptionFilter();

            // 3) Auto reg. Repositories, Services, Mappers
            services
                .AddKumadioRepositories()
                .AddKumadioServices()
                .AddKumadioMappers();

            // 4) Configuration Settings
            services.Configure<AppSettings>(
                    configuration.GetSection("ApplicationSettings")
            );
            services.Configure<OpenAISettings>(
                    configuration.GetSection("OpenAI")
            );

            // 5) Newtonsoft JSON / Memory Cache
            services
                .AddControllers()
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                });
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();

            services.AddMemoryCache();

            // 6) Authentication
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddCookie(x =>
            {
                x.Cookie.Name = "token";

            })
            .AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["ApplicationSettings:Secret"])),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
                x.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        context.Token = context.Request.Cookies["X-Access-Token"];
                        return Task.CompletedTask;
                    }
                };
            });

            // 7) Authorization policies
            services.AddAuthorization(options =>
            {
                options.AddPolicy("ListenerNoSubscription", policy => policy.RequireRole("LISTENER_NO_SUBSCRIPTION"));
            });

            // 8) CORS
            services.AddCors(options =>
            {
                options.AddPolicy(name: "CorsPolicy", builder =>
                {
                    builder.WithOrigins(configuration["ApplicationSettings:ClientUrl"])
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                });
            });

            // 9) Https Redirection
            services.AddHttpsRedirection(options =>
            {
                options.RedirectStatusCode = StatusCodes.Status307TemporaryRedirect;
                options.HttpsPort = 5001; // Adjust according to your setup
            });

            // 10) Http context 
            services.AddHttpContextAccessor();

            return services;
        }
    }
}
