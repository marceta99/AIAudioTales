using Microsoft.AspNetCore.Mvc;

namespace Kumadio.Web.Common
{
    public static class MessageResponse
    {
        public static ActionResult Ok(string message)
        {
            return new OkObjectResult(new { message });
        }
    }

}
