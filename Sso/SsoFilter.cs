using Api.Config.Cache;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using NLog;
using SSO.Client;
using System;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Api.Config.Sso
{
    public abstract class SsoFilter : AuthAttribute
    {
        private readonly ISsoHandler _ssoHandler;
        private readonly SsoOptions _options;
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
       
        public SsoFilter(ISsoHandler ssoHandler)
        {
            _ssoHandler = ssoHandler;
            _options = ssoHandler.GetOptions();
        }
        /// <summary>
        /// 获取CookieID
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public abstract string GetCookieID(HttpContext context);

        /// <summary>
        /// 通过验证
        /// </summary>
        /// <param name="cookie"></param>
        /// <returns></returns>
        public abstract Task ValidateComplate(SsoCookie cookie);

        /// <summary>
        /// 退出登录
        /// </summary>
        /// <param name="cookie"></param>
        /// <returns></returns>
        public abstract Task LogoutComplate(SsoCookie cookie);
        
        /// <summary>
        /// 访问控制验证
        /// </summary>
        /// <param name="access_token"></param>
        /// <returns></returns>
        public virtual bool AccessValidate(HttpContext context, string access_token) { return false; }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);

            if (WhiteListContain)
            {
                return;
            }

            if (context.HttpContext.PassSso())
            {
                return;
            }

            if (Attrs.Any(a => a.GetType().Equals(typeof(NoSsoAttribute))))
            {
                return;
            }

            var request = context.HttpContext.GetRequest(_options.Mode);
            if (request.Query.ContainsKey(HttpExtention.ACCESS_TOKEN_KEY))
            {
                var access_token = request.Query[HttpExtention.ACCESS_TOKEN_KEY][0];
                if ((access_token != context.HttpContext.Response.GetToken() && AccessValidate(context.HttpContext, access_token))
                    || AccessTokens.Contains(access_token))
                {
                    context.HttpContext.SetToken(access_token);
                    return;
                }      
            }
            request.ClientIP = ClientIp;
            request.Ticket = GetCookieID(context.HttpContext);
            if (string.IsNullOrEmpty(request.Ticket) && _ssoHandler.IsLogout(request.OriginPath))
            {
                request.Ticket = context.HttpContext.Request.Query[SsoParameter.TICKET];
            }
            _ssoHandler.SetRequest(request);//设置请求           

            var mapping_url = _ssoHandler.GetOptions().GetBaseURL(request.OriginHost, true);
                        
            _logger.Debug($"origin={ context.HttpContext.Request.Headers["origin"]}");
            _logger.Debug($"referer={ context.HttpContext.Request.Headers["referer"]}");
            _logger.Debug($"RequestHost={request.OriginHost},映射地址：" + mapping_url);

            request.CallBack.Redirect += new Action<string, bool, SsoMode>((url, repeat_check, mode) =>
            {
                if (repeat_check)
                {                    
                    var key = Convert.ToBase64String(Encoding.UTF8.GetBytes(request.ClientIP + request.GetURL()));
                    var create_time = CacheUnit.Current.GetAsync<DateTime?>(key).GetAwaiter().GetResult();
                    if (create_time != null)
                    {
                        url = "";
                    }
                    else
                    {
                        CacheUnit.Current.SetAsync(key, DateTime.Now, TimeSpan.FromSeconds(5)).GetAwaiter().GetResult();
                    }
                }

                if (context.HttpContext.Response.HasStarted)
                {
                    return;
                }

                if (request.GetURL() == url && context.HttpContext.Response.CheckSso())
                {
                    return;
                }

                if (string.IsNullOrEmpty(url))
                {
                    context.HttpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                    context.Result = new NoContentResult();
                    return;
                }

                context.HttpContext.Response.AddHeader("redirect-url", url);
                if (mode == SsoMode.Proxy)
                {
                    //跳转  
                    context.HttpContext.Response.AddHeader("Access-Control-Expose-Headers", "redirect-url", true, ",");
                    context.HttpContext.Response.StatusCode = (int)HttpStatusCode.Redirect;
                    context.Result = new JsonResult(context.HttpContext.Response.StatusCode);
                    return;
                }
                else
                {                    
                    context.HttpContext.Response.StatusCode = (int)HttpStatusCode.Redirect;   
                }
            });

            request.CallBack.Logout += new Action<SsoCookie>((cookie) =>
            {
                if (context.HttpContext.Response.HasStarted)
                {
                    return;
                }
                if (cookie != null)
                {
                    _logger.Debug($"【退出登录】清理本地Session，id={ cookie.ID }");
                    LogoutComplate(cookie).GetAwaiter().GetResult();
                }
            });

            if (_ssoHandler.Exist(request.Ticket))
            {
                //已经通过验证
                if (_ssoHandler.IsLogout(request.OriginPath))
                {                    
                    bool redirect_flag = true;
                    redirect_flag = context.HttpContext.Request.Query[SsoParameter.Redirect].ToString() != "no";
                    _logger.Debug($"【退出登录】开始执行，票据：{ request.Ticket }，redirect_flag={ redirect_flag }");
                    _ssoHandler.Logout(request.Ticket, redirect_flag);
                    _logger.Debug($"【退出登录】执行完成，票据：{ request.Ticket }");
                }
                else
                {
                    context.HttpContext.Response.SetSsoPass();
                }
                return;
            }

            request.CallBack.Validate += new Action<SsoCookie>((cookie) =>
            {
                if (context.HttpContext.Response.HasStarted)
                {
                    return;
                }
                if (cookie != null && !string.IsNullOrEmpty(cookie.ID))
                {                    
                    context.HttpContext.Response.SetSsoPass();                    
                    try
                    {
                        ValidateComplate(cookie).GetAwaiter().GetResult();                        
                    }
                    catch (Exception ex)
                    {
                        _logger.Error(ex.Message, ex.StackTrace);
                    }
                }
            });
            try
            {
                //Cas验证
                _ssoHandler.Validate(true);
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex.StackTrace);
            }
        }
    }
}