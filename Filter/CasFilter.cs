using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using NLog;
using SSO.Client;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Api.Config
{
    public class CasFilter : AuthAttribute
    {
        private readonly ICasHandler _casHandler;
        private readonly CasOptions _options;
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
       
        public CasFilter(ICasHandler casHandler)
        {
            _casHandler = casHandler;
            _options = casHandler.GetOptions();
        }

        public virtual Task ValidateComplate(CasCookie cookie) { return Task.CompletedTask; }

        public virtual Task LogoutComplate(CasCookie cookie) { return Task.CompletedTask; }
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

            if (context.HttpContext.PassCas())
            {
                return;
            }

            if (Attrs.Any(a => a.GetType().Equals(typeof(NoCasAttribute))))
            {
                return;
            }

            var request = context.HttpContext.GetCasRequest(_options.Mode);
            if (request.Query.ContainsKey(HttpExtention.ACCESS_TOKEN_KEY))
            {
                var access_token = request.Query[HttpExtention.ACCESS_TOKEN_KEY];
                if ((access_token != context.HttpContext.Response.GetToken() && AccessValidate(context.HttpContext, access_token))
                    || AccessTokens.Contains(access_token))
                {
                    context.HttpContext.SetToken(access_token);
                    return;
                }      
            }
            request.ClientIP = ClientIp;
            _casHandler.SetRequest(request);//设置请求
            request.RequestHost = context.HttpContext.GetBaseUrl();

            var mapping_url = _casHandler.GetOptions().GetBaseURL(request.RequestHost, true);
                        
            _logger.Debug($"origin={ context.HttpContext.Request.Headers["origin"]}");
            _logger.Debug($"referer={ context.HttpContext.Request.Headers["referer"]}");
            _logger.Debug($"RequestHost={request.RequestHost},映射地址：" + mapping_url);

            request.CallBack.Redirect += new Action<string>((url) =>
            {
                if (context.HttpContext.Response.HasStarted)
                {
                    return;
                }
                if (_options.Mode == CasMode.Proxy)
                {
                    //跳转                    
                    context.HttpContext.Response.AddHeader("redirect-url", url);
                    context.HttpContext.Response.AddHeader("Access-Control-Expose-Headers", "redirect-url", true, ",");
                    context.HttpContext.Response.StatusCode = (int)HttpStatusCode.Redirect;
                    context.Result = new JsonResult(context.HttpContext.Response.StatusCode);
                }
                else
                {
                    //携带请求Body
                    context.HttpContext.Response.Redirect(url);
                }
            });
                        
            var token = context.HttpContext.GetToken();
            if (string.IsNullOrEmpty(token) && _casHandler.IsLogout(context.HttpContext.Request.Path))
            {
                token = context.HttpContext.Request.Query[CasParameter.TICKET];
            }
            if (_casHandler.Exist(token))
            {
                //已经通过验证
                if (_casHandler.IsLogout(context.HttpContext.Request.Path))
                {
                    request.CallBack.Logout += new Action<CasCookie>(async (cookie) =>
                    {
                        if (cookie != null)
                        {
                            if (context.HttpContext.Response.HasStarted)
                            {
                                return;
                            }
                            await LogoutComplate(cookie);
                        }
                    });
                    context.HttpContext.ClearToken();
                    _casHandler.Logout(token);
                }
                else
                {
                    context.HttpContext.Response.SetCasPass();
                }
                return;
            }

            request.CallBack.Validate += new Action<CasCookie>(async (cookie) =>
            {
                if (cookie != null && !string.IsNullOrEmpty(cookie.ID))
                {
                    if (context.HttpContext.Response.HasStarted)
                    {
                        return;
                    }
                    context.HttpContext.Response.SetCasPass();
                    try
                    {
                        await ValidateComplate(cookie);
                        //验证
                        context.HttpContext.SetToken(cookie.ID);
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
                _casHandler.Validate(true);
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex.StackTrace);
            }
        }
    }
}