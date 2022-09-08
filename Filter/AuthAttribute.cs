using Api.Config.Filter;
using Api.Config.Setting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Config
{
    public abstract class AuthAttribute : IActionFilter
    {
        /// <summary>
        /// 白名单包含？
        /// </summary>
        internal bool WhiteListContain { get; set; }
        /// <summary>
        /// 包含NoAuthorization特性
        /// </summary>
        internal bool NoAuthAttr { get; set; }
        /// <summary>
        /// 请求客户端IP
        /// </summary>
        internal string ClientIp { get; set; }        

        internal object[] Attrs { get; set; }

        public virtual void OnActionExecuted(ActionExecutedContext context)
        {
            
        }

        public virtual void OnActionExecuting(ActionExecutingContext context)
        {
            ClientIp = context.HttpContext.Connection.RemoteIpAddress.ToString();
            var hosts = AppSetting.GetSetting("WhiteList");
            if (hosts != null && hosts.Length > 0)
            {
                if (hosts.Split(',').Contains(ClientIp))
                {
                    WhiteListContain = true;
                }
            }
            
            var isDefined = false;
            var controllerActionDescriptor = context.ActionDescriptor as ControllerActionDescriptor;
            if (controllerActionDescriptor != null)
            {
                Attrs = controllerActionDescriptor.MethodInfo.GetCustomAttributes(inherit: true);

                isDefined = Attrs.Any(a => a.GetType().Equals(typeof(NoAuthorizationAttribute)));
            }
            else
            {
                Attrs = new object[0];
            }
            if (isDefined)
            {
                NoAuthAttr = true;
            }
            else
            {
                var open = Attrs.FirstOrDefault(t => t.GetType().Equals(typeof(OpenAttribute)));
                if (open != null)
                {
                    var _apps = ((OpenAttribute)open).GetApps();
                    if (context.HttpContext.OpenCheck(_apps))
                    {
                        NoAuthAttr = true;
                    }
                }
            }
            
        }        
    }
}
