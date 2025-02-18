using Kumadio.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Kumadio.Web.Attributes.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class RequireCurrentUserAttribute : ActionFilterAttribute
    {
        // Runs before the controller action
        public override void OnActionExecuting(ActionExecutingContext context)
        { 
            var user = context.HttpContext.Items["CurrentUser"] as User;
            
            if(user == null)
            {
                // return 401 if there is no user in context
                context.Result = new UnauthorizedResult();
            }

            // If user is present, do nothing—action continues
        }
    }
}
