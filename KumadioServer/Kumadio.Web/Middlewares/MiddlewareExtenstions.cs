using Microsoft.Extensions.FileProviders;

namespace Kumadio.Web.Middlewares
{
    public static class MiddlewareExtenstions
    {
        public static WebApplication UseCustomMiddlewares(this WebApplication app)
        {
            app.UseMiddleware<GlobalExceptionHandlerMiddleware>();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseCors("CorsPolicy");

            // Enable HTTPS redirection
            app.UseHttpsRedirection();

            // Serve files from "uploads" directory
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(
                    Path.Combine(Directory.GetCurrentDirectory(), "uploads")),
                RequestPath = "/uploads"
            });

            app.UseAuthentication();

            app.UseMiddleware<RequestLoggingMiddleware>();

            app.UseAuthorization();

            return app;
        }
    }
}
