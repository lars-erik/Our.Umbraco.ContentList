using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Our.Umbraco.ContentList.Controllers;

public class LowerCaseInputJsonAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var request = context.HttpContext.Request;
        if (request.ContentType == "application/json")
        {
            request.EnableBuffering();
        }
    }
}