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
                x.RequireHttpsMetadata = false; // only for dev; set to true in production if using HTTPS
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["ApplicationSettings:Secret"])),
                    ValidateIssuer = true,
                    ValidIssuer = configuration["ApplicationSettings:Issuer"], // Issuer is authority that issues the token. In your case, that’s your .NET API
                    ValidateAudience = true,
                    ValidAudience = configuration["ApplicationSettings:Audience"], // Audience is intended recipient of the token, again the resource server that’s validating the token.
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero  // optional: default 5 min clock skew
                };
                x.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        // 1) Extract token from http only cookie if this is request from web application
                        var tokenFromCookie = context.Request.Cookies["X-Access-Token"];
                        if (!string.IsNullOrEmpty(tokenFromCookie))
                        {
                            context.Token = tokenFromCookie;
                        }
                        else
                        {
                            // 2) Extract token from authorization header if this is request from mobile app
                            var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();
                            if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                            {
                                // Extract token after "Bearer "
                                context.Token = authHeader.Substring("Bearer ".Length).Trim();
                            }
                        }
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
                    var clientUrls = configuration.GetSection("ApplicationSettings:ClientUrls").Get<string[]>();
                    builder.WithOrigins(clientUrls)
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
