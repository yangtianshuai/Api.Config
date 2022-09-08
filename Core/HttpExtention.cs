﻿using Api.Config.Setting;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace Api.Config
{
    public static partial class HttpExtention
    {
        /// <summary>
        /// 获取基本请求路径
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static string GetBaseUrl(this HttpContext context)
        {
            var http = $"{context.Request.Scheme}://{context.Request.Host}";
            var _virtual = AppSetting.GetSetting("Virtual");
            if (_virtual != null && _virtual.Length > 0)
            {
                http = http + _virtual;
            }
            return http;
        }

        public static Dictionary<string, string> GetQueryString(this HttpContext context,string url)
        {
            var query = new Dictionary<string, string>();
            if (!url.Contains("?"))
            {
                return query;
            }
            var _params = url.Split('?')[1].Split('&');
            foreach (var _param in _params)
            {
                if (!_param.Contains("="))
                {
                    continue;
                }
                var array = _param.Split('=');
                if (!query.ContainsKey(array[0]))
                {
                    //加载查询条件请求参数（URL中Param传参）
                    query.Add(array[0], array[1]);
                }
            }
            return query;
        }

        /// <summary>
        /// 获取远端连接IP
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static string GetRemoteIp(this HttpContext context)
        {
            return context.Connection.RemoteIpAddress.ToString();
        }

        /// <summary>
        /// 获取用户代理信息
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static string GetUserAgent(this HttpRequest request)
        {
            return request.Headers["User-Agent"].ToString();
        }

        public static Dictionary<string, int> GetBrowser(this HttpRequest request)
        {
            var user_agent = GetUserAgent(request);
            var browsers = new Dictionary<string, int>();
            var browser_array = user_agent.Split(' ');
            foreach (var brower_text in browser_array)
            {
                if (!brower_text.Contains("/"))
                {
                    continue;
                }
                var browser = brower_text.Split('/');
                var index = browser[1].IndexOf(".");
                try
                {
                    var version = int.Parse(browser[1].Substring(0, index));
                    browsers.Add(browser[0], version);
                }catch
                {}                
            }
            return browsers;
        }

        public static int? GetBrowserVersion(this HttpRequest request,string type)
        {
            var browsers = GetBrowser(request);
            if(browsers.ContainsKey(type))
            {
                return browsers[type];
            }
            return null;
        }

        private static string _key = "setookie";

        public static void SetCookie2(this HttpResponse response, string key, string value, CookieOptions options)
        {
            string content = $"{key}={value}";
            if (response.Headers.ContainsKey(_key))
            {
                string _value = response.Headers[_key].ToString();
                if (!_value.Contains(key))
                {
                    content += $";{_value}";
                }
            }
            else if (options != null)
            {
                if (!string.IsNullOrEmpty(options.Path))
                {
                    content += $";path={options.Path}";
                }
                if (!string.IsNullOrEmpty(options.Domain))
                {
                    content += $";domain={options.Domain}";
                }
                if (options.SameSite != SameSiteMode.None)
                {
                    content += $";samesite={options.SameSite.ToString().ToLower()}";
                }
                if (options.Expires != null)
                {
                    //content += $";expires={options.Expires.Value.ToUniversalTime().ToString("ddd, dd MMM yyyy HH:mm:ss 'GMT'")}";
                }
                if (options.HttpOnly)
                {
                    //content += $";httponly";
                }
            }
            response.Headers[_key] = content;
        }
    }
}