using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using SSO.Client;
using System;
using System.Collections.Generic;
using System.IO;
using System.Web;

namespace Api.Config
{
    public static class CasExtensions
    {
        /// <summary>
        /// 添加Cas服务
        /// </summary>
        /// <param name="services"></param>
        /// <param name="option">CAS参数</param>
        /// <returns></returns>
        public static IServiceCollection AddCas(this IServiceCollection services, Action<CasOptions> option)
        {
            var options = new CasOptions();
            option?.Invoke(options);
            services.AddSingleton(typeof(CasOptions), options);
            //设置注入项目
            if (options.LogoutPath == null)
            {               
                options.LogoutPath = "/sso/user/Logout";
                MvcRouter.Add(options.LogoutPath, context =>
                {                    
                    context.ClearToken();
                });
            }           
            services.AddScoped<ICasHandler, CasHandler>();
            return services;
        }

        public static List<IPMapping> AddIpMappings(this CasOptions option, IPMapping[] mappings)
        {
            if (option.IPMappings == null)
            {
                option.IPMappings = new List<IPMapping>();
            }
            if (mappings == null)
            {
                return option.IPMappings;
            }            
            foreach(var mapping in mappings)
            {
                var _mapping = option.IPMappings.Find(t => t.server_ip == mapping.server_ip && t.base_url == mapping.base_url);
                if (_mapping == null)
                {
                    option.IPMappings.Add(mapping);
                }
            }       
            return option.IPMappings;
        }

        public static List<IPMapping> AddIpMapping(this CasOptions option, IPMapping mapping)
        {
            if (option.IPMappings == null)
            {
                option.IPMappings = new List<IPMapping>();
            }
            if (mapping == null)
            {
                return option.IPMappings;
            }
            var _mapping = option.IPMappings.Find(t => t.server_ip == mapping.server_ip && t.base_url == mapping.base_url);
            if (_mapping == null)
            {
                option.IPMappings.Add(mapping);
            }
            return option.IPMappings;
        }

        /// <summary>
        /// 获取Cas请求
        /// </summary>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        public static CasRequest GetCasRequest(this HttpContext httpContext, CasMode casMode)
        {
            var request = new CasRequest
            {
                Scheme = httpContext.Request.Scheme,
                Host = httpContext.Request.Host.ToString(),
                Path = httpContext.Request.Path
            };            
            if (casMode == CasMode.Proxy)
            {
                var url = httpContext.Request.Headers["url"].ToString();                
                if (string.IsNullOrEmpty(url))
                {
                    url = httpContext.Request.Headers["referer"].ToString();
                }
                if (!string.IsNullOrEmpty(url))
                {
                    var uri = new Uri(HttpUtility.UrlDecode(url));
                    request.Scheme = uri.Scheme;
                    request.Host = uri.Host;
                    request.Port = uri.Port;
                    request.Path = uri.PathAndQuery;
                    request.Query = uri.Query.GetQuery();
                }                
            }
            else
            {
                var querys = httpContext.Request.Query;
                foreach (var query in querys)
                {
                    //加载查询条件请求参数（URL中Param传参）
                    if (!request.Query.ContainsKey(query.Key))
                    {
                        request.Query.Add(query.Key, query.Value);
                    }
                }
                var cookies = httpContext.Request.Cookies;
                foreach (var cookie in cookies)
                {
                    //加载查询条件请求参数（URL中Param传参）
                    if (!request.Query.ContainsKey(cookie.Key))
                    {
                        request.Cookie.Add(cookie.Key, cookie.Value);
                    }
                }
                if (httpContext.Request.ContentLength != null)
                {
                    try
                    {
                        byte[] buffer = new byte[httpContext.Request.Body.Length];
                        httpContext.Request.Body.Read(buffer, 0, buffer.Length);
                        httpContext.Request.Body.Seek(0, SeekOrigin.Begin);
                        request.Body = buffer;//Body
                    }
                    catch { }
                }
            }
            return request;
        }

        private static string no_cas = "no-cas";

        /// <summary>
        /// 通过Cas验证
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static bool PassCas(this HttpContext context)
        {
            var cas = "";
            if (context.Request.Cookies.ContainsKey(no_cas))
            {
                cas = context.Request.Cookies[no_cas];
            }
            else if (context.Response.Headers.ContainsKey(no_cas))
            {
                cas = context.Request.Cookies[no_cas];
            }

            return cas == "true";
        }

        /// <summary>
        /// 无需Cas验证
        /// </summary>       
        /// <returns></returns>
        public static void NoCas(this HttpContext context)
        {
            if(PassCas(context))
            {
                return;
            }
            var option = new CookieOptions
            {
                Path = "/",
                Expires = DateTimeOffset.Now.AddYears(100),
                SameSite = SameSiteMode.None,
                HttpOnly = true
            };
            if (context.Request.Scheme.ToLower() == "https")
            {                
                option.Secure = true;
            }
            else
            {                
                if (context.Request.GetBrowserVersion("Chrome") > 79)
                {
                    context.Response.SetCookie2(no_cas, "true", option);
                }
            }
            context.Response.Cookies.Append(no_cas, "true", option);
        }
        
        /// <summary>
        /// 清除Cas验证
        /// </summary>       
        /// <returns></returns>
        public static void ClearCas(this HttpContext context)
        {
            context.Response.Cookies.Delete(no_cas);
        }
    }
}