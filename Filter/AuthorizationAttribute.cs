using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Api.Config
{
    /// <summary>
    /// 需要验证
    /// </summary>
    public class AuthorizationAttribute : AuthAttribute
    {
        private ISessionService _session;
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public AuthorizationAttribute(ISessionService session):base()
        {
            _session = session;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);

            if (this.NoAuthAttr)
            {                
                return;
            }

            if (this.WhiteListContain)
            {
                if (!Attrs.Any(a => a.GetType().Equals(typeof(SessionAuthAttribute))))
                {                   
                    return;
                }
            }

            var token = context.HttpContext.GetToken();
            if (token == null)
            {
                if (context.HttpContext.PassSso())
                {
                    context.HttpContext.ClearSsoCookie();
                    return;
                }
                context.HttpContext.NoAuthorization();
                context.Result = new JsonResult("No Authorization");
                _logger.Debug("No Authorization，请求未发现token，IP：" + ClientIp);
                return;
            }
            else
            {
                if(context.HttpContext.Response.CheckSso())
                {
                    return;
                }
            }
            var session = _session.GetAsync<Session>(token).GetAwaiter().GetResult();
            if (session == null)
            {                
                context.HttpContext.NoAuthorization();
                context.Result = new JsonResult("No Authorization 2");
                _logger.Debug("No Authorization（服务器token不存在），IP：" + ClientIp);
                return;
            }

            var span = DateTime.Now - session.Create_Time;
            if (span.TotalMinutes > 10)
            {
                session.Create_Time = DateTime.Now;
                _session.Update(session);
            }
            var roles = Attrs.Where(t => t.GetType().Equals(typeof(RolesAttribute)));
            var list = new List<string>();
            foreach (var role in roles)
            {
                var _roles = ((RolesAttribute)role).GetRoles();
                foreach (var _role in _roles)
                {
                    if (!list.Contains(_role))
                    {
                        list.Add(_role);
                    }
                }
            }
            if (list.Count > 0 && session.Roles != null)
            {
                if (list.Where(t => session.Roles.Contains(t)).Count() == 0)
                {
                    context.HttpContext.NoAuthorization();
                    context.Result = new JsonResult("No Role Authorization");
                    _logger.Debug($"No Role Authorization，IP：{ClientIp},路由：{context.HttpContext.Request.Path}");
                    return;
                }
            }
        }
    }
}