using System.Net;
using System.Text.Json;

public class GlobalExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;

    public GlobalExceptionHandlerMiddleware(
        RequestDelegate next,
        ILogger<GlobalExceptionHandlerMiddleware> logger)
    {
        _next = next;
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
                Error = ex.Message,  // For production, hide or sanitize this
                StackTrace = ex.StackTrace //  omit in production or log separately
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
