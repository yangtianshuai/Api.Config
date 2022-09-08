using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Api.Config
{
    public class MvcRouter : IRouter
    {
        private static Dictionary<string, Action<HttpContext>> routers = new Dictionary<string, Action<HttpContext>>();
        public VirtualPathData GetVirtualPath(VirtualPathContext context)
        {
            return null;
        }

        public static void Add(string key, Action<HttpContext> action)
        {
            key = key.ToLower();
            if (!routers.ContainsKey(key))
            {
                routers.Add(key, action);
            }
        }

        public Task RouteAsync(RouteContext context)
        {
            var key = context.HttpContext.Request.Path.Value.TrimEnd('/');
            key = key.ToLower();
            var list = new List<string>(routers.Keys);
            key = list.Find(t => key.LastIndexOf(t) > 0);
            if (key != null)
            {              
                context.Handler = async ctx =>
                {
                    //具体处理规则
                    routers[key](ctx);
                };               
            }
            return Task.CompletedTask;
        }
    }
}
