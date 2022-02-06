using Microsoft.AspNetCore.Mvc.Filters;

namespace Totem;

public class ErrorInfoActionFilterAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext actionContext)
    {
        var modelState = actionContext.ModelState;

        if(!modelState.IsValid)
        {
            actionContext.Result = new ErrorInfoResult(
                from value in modelState.Values
                from error in value.Errors
                select new ErrorInfo(error.ErrorMessage));
        }
    }
}
