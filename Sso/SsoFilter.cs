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
    public class SsoFilter : AuthAttribute
    {
        private readonly ISsoHandler _ssoHandler;
        private readonly SsoOptions _options;
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
       
        public SsoFilter(ISsoHandler ssoHandler)
        {
            _ssoHandler = ssoHandler;
            _options = ssoHandler.GetOptions();
        }

        public virtual string GetCookieID() { return ""; }

        /// <summary>
        /// 通过验证
        /// </summary>
        /// <param name="cookie"></param>
        /// <returns></returns>
        public virtual Task ValidateComplate(SsoCookie cookie) { return Task.CompletedTask; }

        /// <summary>
        /// 退出登录
        /// </summary>
        /// <param name="cookie"></param>
        /// <returns></returns>
        public virtual Task LogoutComplate(SsoCookie cookie) { return Task.CompletedTask; }
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
            _ssoHandler.SetRequest(request);//设置请求
            request.RequestHost = context.HttpContext.GetBaseUrl();

            var mapping_url = _ssoHandler.GetOptions().GetBaseURL(request.RequestHost, true);
                        
            _logger.Debug($"origin={ context.HttpContext.Request.Headers["origin"]}");
            _logger.Debug($"referer={ context.HttpContext.Request.Headers["referer"]}");
            _logger.Debug($"RequestHost={request.RequestHost},映射地址：" + mapping_url);

            request.CallBack.Redirect += new Action<string, bool>((url, repeat_check) =>
            {
                if (repeat_check)
                {
                    var temp = request.GetURL();
                    var key = Convert.ToBase64String(Encoding.UTF8.GetBytes(request.ClientIP + request.GetURL()));
                    var create_time = CacheUnit.Current.GetAsync<DateTime?>(key).GetAwaiter().GetResult();
                    if (create_time != null)
                    {
                        url = "";
                    }
                    else
                    {
                        CacheUnit.Current.SetAsync(key, DateTime.Now, TimeSpan.FromSeconds(10)).GetAwaiter().GetResult();
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

                if (_options.Mode == SsoMode.Proxy)
                {
                    //跳转                    
                    context.HttpContext.Response.AddHeader("redirect-url", url);
                    context.HttpContext.Response.AddHeader("Access-Control-Expose-Headers", "redirect-url", true, ",");
                    context.HttpContext.Response.StatusCode = (int)HttpStatusCode.Redirect;
                    context.Result = new JsonResult(context.HttpContext.Response.StatusCode);
                    return;
                }
                else
                {
                    //携带请求Body
                    context.HttpContext.Response.Redirect(url);
                }
            });
                        
            var token = GetCookieID();
            if (string.IsNullOrEmpty(token) && _ssoHandler.IsLogout(context.HttpContext.Request.Path))
            {
                token = context.HttpContext.Request.Query[SsoParameter.TICKET];
            }
            if (_ssoHandler.Exist(token))
            {
                //已经通过验证
                if (_ssoHandler.IsLogout(context.HttpContext.Request.Path))
                {
                    request.CallBack.Logout += new Action<SsoCookie>((cookie) =>
                    {
                        if (cookie != null)
                        {
                            if (context.HttpContext.Response.HasStarted)
                            {
                                return;
                            }
                            LogoutComplate(cookie).GetAwaiter().GetResult();
                        }
                    });
                    bool redirect_flag = true;
                    redirect_flag = context.HttpContext.Request.Query[SsoParameter.Redirect] != "no";
                    _ssoHandler.Logout(token, redirect_flag);
                }
                else
                {
                    context.HttpContext.Response.SetSsoPass();
                }
                return;
            }

            request.CallBack.Validate += new Action<SsoCookie>((cookie) =>
            {
                if (cookie != null && !string.IsNullOrEmpty(cookie.ID))
                {
                    if (context.HttpContext.Response.HasStarted)
                    {
                        return;
                    }
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