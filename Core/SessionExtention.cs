using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Api.Config
{
    /// <summary>
    /// SessionExtention
    /// </summary>
    public static class SessionExtention
    {
        private static string token_key = "token";
        /// <summary>
        /// 添加内存型Session
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddMemorySession(this IServiceCollection services)
        {
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            //Session服务
            services.AddSingleton<ISessionService, SessionService>();
            services.AddSingleton<ServerSession>();

            return services;
        }

        /// <summary>
        ///  添加内存型Session
        /// </summary>
        /// <param name="services"></param>
        /// <param name="option"></param>
        /// <returns></returns>
        public static IServiceCollection AddMemorySession(this IServiceCollection services
            , Action<SessionOption> option)
        {
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            if (option != null)
            {
                var sessionOption = new SessionOption();
                option(sessionOption);
                if (sessionOption.TokenKey != null && sessionOption.TokenKey.Length > 0)
                {
                    token_key = sessionOption.TokenKey;
                }
                if (sessionOption.Service != null)
                {
                    services.AddSingleton<ServerSession>();

                    if (sessionOption.TimeSpan != null && sessionOption.TimeSpan != TimeSpan.MinValue)
                    {
                        ServerSession.Span = sessionOption.TimeSpan;
                    }
                    else
                    {
                        ServerSession.Span = TimeSpan.FromHours(8);
                    }
                    //自定义Session服务
                    services.AddSingleton(typeof(ISessionService), sessionOption.Service);
                    return services;
                }
            }
            return AddMemorySession(services);
        }

        /// <summary>
        /// HasToken
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static bool HasToken(this HttpContext context)
        {
            return context.Request.Cookies.ContainsKey(token_key) || context.Request.Headers.ContainsKey(token_key);
        }

        /// <summary>
        ///
        /// </summary>       
        /// <returns></returns>
        public static string GetKey(this HttpContext context)
        {
            return token_key;
        }        

        /// <summary>
        /// 获取Token
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static string GetToken(this HttpContext context)
        {
            string token = null;
            if (context.Request.Headers.ContainsKey(token_key))
            {
                token = context.Request.Headers[token_key].ToString();
            }            
            if (string.IsNullOrEmpty(token))
            {
                if (context.Request.Cookies.ContainsKey(token_key))
                {
                    token = context.Request.Cookies[token_key].ToString();
                }
            }
           
            if (token != null && token == "null")
            {
                return null;
            }
            return token;
        }

        public static string GetToken(this HttpResponse response)
        {           
            if (response.Headers.ContainsKey(token_key))
            {
                return response.Headers[token_key].ToString();
            }
            return null;
        }        

        public static void SetToken(this HttpResponse response, string token)
        {
            AddHeader(response, token_key, token);
            AddHeader(response, "Access-Control-Expose-Headers", token_key, true, ",");
        }

        private static string sso_pass_key = "sso_pass";
        public static void SetSsoPass(this HttpResponse response)
        {
            AddHeader(response, sso_pass_key, "true");
            AddHeader(response, "Access-Control-Expose-Headers", sso_pass_key, true, ",");
        }
        public static bool CheckSso(this HttpResponse response)
        {
            var cas = response.Headers[sso_pass_key];
            return cas == "true";
        }

        public static void AddHeader(this HttpResponse response, string header, string value, bool check = false, string split = null)
        {
            if (response.Headers.ContainsKey(header))
            {
                if (check)
                {
                    if (!response.Headers[header].ToString().Contains(value))
                    {
                        response.Headers[header] = response.Headers[header] + split + value;
                    }
                }
                else
                {
                    response.Headers[header] = value;
                }
            }
            else
            {
                response.Headers.Add(header, value);
            }
        }

        public static void SetToken(this HttpRequest request, string token)
        {
            if (request.Headers.ContainsKey(token_key))
            {
                request.Headers[token_key] = token;
            }
            else
            {
                request.Headers.Add(token_key, token);
            }
        }

        /// <summary>
        /// 无法认证，返回401
        /// </summary>
        /// <param name="context"></param>       
        public static void NoAuthorization(this HttpContext context)
        {
            ClearToken(context);
            context.Response.StatusCode = 401;
        }

        public static void ClearToken(this HttpContext context)
        {
            //清除Cookie            
            context.Response.Cookies.Delete(token_key);
        }

        public static bool IsCrossDomain(this HttpContext context)
        {
            return GetOrigin(context) != context.Request.Host.Host;
        }

        public static string GetOrigin(this HttpContext context)
        {
            var url = context.Request.Headers["referer"].ToString();
            if (string.IsNullOrEmpty(url))
            {
                url = context.Request.Headers["origin"].ToString();
            }
            if (!string.IsNullOrEmpty(url))
            {
                var uri = new Uri(url);
                return uri.Host;
            }
            return context.Request.Host.Host;
        }

        /// <summary>
        /// 设置返回HTTP头Token
        /// </summary>
        /// <param name="context"></param>
        /// <param name="token"></param>
        public static void SetToken(this HttpContext context, string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return;
            }
            var option = new CookieOptions
            {
                Path = "/",
                Expires = DateTimeOffset.Now.AddYears(100),               
                HttpOnly = true
            };
            
            if (context.Request.Scheme.ToLower() == "https")
            {
                option.SameSite = SameSiteMode.None;
                option.Secure = true;
            }
            context.Response.SetToken(token);
            context.Response.Cookies.Append(token_key, token, option);
        }

        /// <summary>
        /// 设置返回HTTP头Token
        /// </summary>
        /// <param name="session"></param>      
        public static void SetToken(this HttpContext context, Session session)
        {
            context.ClearToken();
            SetToken(context, session.Token);
            if (session.Roles != null && session.Roles.Count > 0)
            {               
                //设置角色
            }
        }
    }
}