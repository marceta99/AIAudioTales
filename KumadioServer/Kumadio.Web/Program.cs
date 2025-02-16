using Kumadio.Web.Middlewares;
using Kumadio.Web.ServiceRegistrations;

var builder = WebApplication.CreateBuilder(args);
ConfigurationManager configuration = builder.Configuration;


// Add user secrets for dev environment
if (builder.Environment.IsDevelopment())
{
    builder.Configuration.AddUserSecrets<Program>();
}

builder.Services.AddCustomServices(configuration);

// Ensure the upload directory exists
UploadFolderHelper.CreateUploadFolder();

var app = builder.Build();

app.UseCustomMiddlewares();

app.MapControllers();

app.Run();