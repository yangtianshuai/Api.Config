using Api.Config.Filter;
using Api.Config.Proxy;
using Api.Config.Setting;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Collections.Generic;
using System.Linq;

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
        internal List<string> AccessTokens { get; set; }

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
            AccessTokens = AppSetting.GetSetting<List<string>>("AccessToken");
            var access_token = context.HttpContext.Request.GetAccessToken();
            if (!string.IsNullOrEmpty(access_token))
            {                               
                if (AccessTokens.Contains(access_token))
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
                var apps = new List<string>();
                bool access_token_flag = true;
                var open = Attrs.FirstOrDefault(t => t.GetType().Equals(typeof(OpenAttribute)));
                if (open != null)
                {
                    var _open = open as OpenAttribute;
                    apps = _open.GetApps();
                    access_token_flag = !_open.AccessToken();
                }
                var route = context.HttpContext.GetRoute();
                if (!string.IsNullOrEmpty(route) && OpenOptions.OpenApps.ContainsKey(route))
                {
                    apps.AddRange(OpenOptions.OpenApps[route]);
                    access_token_flag = access_token_flag || context.HttpContext.AccessCheck(route);
                }

                if (access_token_flag && context.HttpContext.OpenCheck(apps))
                {
                    NoAuthAttr = true;
                    WhiteListContain = true;
                }
            }          
        }        
    }
}
