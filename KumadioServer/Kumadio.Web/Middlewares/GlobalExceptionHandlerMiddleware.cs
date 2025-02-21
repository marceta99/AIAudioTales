using System.Net;
using System.Text.Json;

public class GlobalExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IHostEnvironment _env;
    private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;

    public GlobalExceptionHandlerMiddleware(
        RequestDelegate next,
        IHostEnvironment env,
        ILogger<GlobalExceptionHandlerMiddleware> logger)
    {
        _next = next;
        _env = env;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            // Move to the next middleware or the endpoint if no exception is thrown
            await _next(context);
        }
        catch (Exception ex)
        {
            // 1) Log the error
            _logger.LogError(ex, "Unhandled exception caught by global handler");

            // For truly unexpected errors, 500 Internal Server Error.
            var statusCode = (int)HttpStatusCode.InternalServerError;

            // Build a standard error response (JSON)
            var problemDetails = new
            {
                Status = statusCode,
                Error = _env.IsDevelopment()
                    ? ex.Message
                    : "An unexpected error occurred on the server.",
                traceId = context.TraceIdentifier,
                stackTrace = _env.IsDevelopment()
                    ? ex.StackTrace
                    : null,
            };

            // 4) Write the response
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = statusCode;

            var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            var json = JsonSerializer.Serialize(problemDetails, options);
            await context.Response.WriteAsync(json);
        }
    }
}
