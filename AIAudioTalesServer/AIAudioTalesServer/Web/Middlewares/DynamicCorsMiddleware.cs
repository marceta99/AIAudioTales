namespace AIAudioTalesServer.Web.Middlewares
{
    public class DynamicCorsMiddleware
    {
        private readonly RequestDelegate _next;

        public DynamicCorsMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            string origin = context.Request.Headers["Origin"];

            // Allow only specific domains or all for development
            if (!string.IsNullOrEmpty(origin) &&
                origin == "https://846b-87-116-166-13.ngrok-free.app")
            {
                context.Response.Headers.Add("Access-Control-Allow-Origin", origin);
                context.Response.Headers.Add("Access-Control-Allow-Credentials", "true");
                context.Response.Headers.Add("Access-Control-Allow-Methods", "GET, POST, OPTIONS, PUT, DELETE");
                context.Response.Headers.Add("Access-Control-Allow-Headers", "Content-Type, Authorization");
            }

            // Handle the preflight request
            if (context.Request.Method == "OPTIONS")
            {
                context.Response.StatusCode = 200;
                await context.Response.CompleteAsync();
                return;
            }

            await _next(context);
        }
    }



}
