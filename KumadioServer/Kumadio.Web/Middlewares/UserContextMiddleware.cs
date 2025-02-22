using Kumadio.Core.Interfaces;
using System.IdentityModel.Tokens.Jwt;

namespace Kumadio.Web.Middlewares
{
    public class UserContextMiddleware
    {
        private readonly RequestDelegate _next;

        public UserContextMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, IAuthService authService)
        {
            var jwtTokenCookie = context.Request.Cookies["X-Access-Token"];
            if (!string.IsNullOrEmpty(jwtTokenCookie))
            {
                try
                {
                    var tokenHandler = new JwtSecurityTokenHandler();
                    var token = tokenHandler.ReadJwtToken(jwtTokenCookie);
                    var emailClaim = token.Claims.FirstOrDefault(c => c.Type == "email");

                    if (emailClaim != null)
                    {
                        var email = emailClaim.Value;
                        var userResult = await authService.GetUserWithEmail(email);
                        if (userResult.IsFailure) return;

                        context.Items["CurrentUser"] = userResult.Value; // Adds the user to HttpContext for later use in controllers
                        
                    }
                }
                catch (Exception ex)
                {
                    // If I add logging of exceptions later to kibana and elastic search I will use this ex instance
                    return; // stop further passing to the next middleware in the pipeline 
                }
            }

            await _next(context); // send http conext to the next middleware in the pipeline
        }
    }
}