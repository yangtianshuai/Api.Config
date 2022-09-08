using Microsoft.AspNetCore.Mvc.Filters;

namespace Api.Config
{
    /// <summary>
    /// 无需验证
    /// </summary>
    public class NoAuthorizationAttribute : ActionFilterAttribute
    {         
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
        }
    }
}