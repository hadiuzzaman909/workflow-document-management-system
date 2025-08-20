using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

public class RequireLoginAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext ctx)
    {
        var token = ctx.HttpContext.Session.GetString("JwtToken");
        if (string.IsNullOrEmpty(token))
            ctx.Result = new RedirectToActionResult("Login", "Admin", null);
    }
}