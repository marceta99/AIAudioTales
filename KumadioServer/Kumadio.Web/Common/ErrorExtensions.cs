using Kumadio.Core.Common;
using Microsoft.AspNetCore.Mvc;

namespace Kumadio.Web.Common
{
    public static class ErrorExtensions
    {
        public static ActionResult ToBadRequest(this Error error)
        {
            return new BadRequestObjectResult(new
            {
                Code = error.Code, 
                Message = error.Message
            });
            
        }
    }
}
