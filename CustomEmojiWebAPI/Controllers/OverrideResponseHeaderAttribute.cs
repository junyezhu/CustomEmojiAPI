namespace CustomEmojiWebAPI.Controllers
{
    using Microsoft.AspNetCore.Mvc.Filters;

    public class OverrideResponseHeaderAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuted(ActionExecutedContext actionExecutedContext)
        {
            actionExecutedContext.HttpContext.Response.Headers["Access-Control-Allow-Origin"] = "*";
            actionExecutedContext.HttpContext.Response.Headers["Access-Control-Allow-Headers"] = "Content-Type";
            actionExecutedContext.HttpContext.Response.Headers["Access-Control-Allow-Methods"] = "GET, POST, PUT, DELETE, OPTIONS";
        }
    }
}
