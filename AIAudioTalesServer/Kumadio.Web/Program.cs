using Kumadio.Web.Middlewares;
using Kumadio.Web.ServiceRegistrations;

var builder = WebApplication.CreateBuilder(args);
ConfigurationManager configuration = builder.Configuration;
configuration.AddJsonFile("appsettings.json");

builder.Services.AddCustomServices(configuration);

// Ensure the upload directory exists
UploadFolderHelper.CreateUploadFolder();

var app = builder.Build();

app.UseCustomMiddlewares();

app.MapControllers();

app.Run();