using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Linq;

namespace Api.Config.Proxy
{
    public class ProxyFilter : IActionFilter
    {
        public void OnActionExecuted(ActionExecutedContext context)
        {
            
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            var controllerActionDescriptor = context.ActionDescriptor as ControllerActionDescriptor;
            if (controllerActionDescriptor != null)
            {
                var Attrs = controllerActionDescriptor.MethodInfo.GetCustomAttributes(inherit: true);
                var proxy = Attrs.FirstOrDefault(t => t.GetType().Equals(typeof(ProxyAttribute)));
                if (proxy != null)
                {
                    var hosts = ((ProxyAttribute)proxy).Get();
                    //任意选择一个路由
                    var step = new Random(hosts.Count).Next();
                    var host = hosts[step];
                    //await context.HttpContext.ProxyAsync(host);
                }
            }
        }
    }
}
