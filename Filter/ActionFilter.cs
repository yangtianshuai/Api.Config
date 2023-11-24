using Api.Config.Setting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using NLog;
using System;
using System.Diagnostics;
using System.Linq;

namespace Api.Config
{
    /// <summary>
    /// 动作过滤器
    /// </summary>
    public class ActionFilter : IActionFilter
    {
        private readonly Logger _logger;
        private Stopwatch watch { get; set; }
        /// <summary>
        /// 请求体中的所有值
        /// </summary>
        private string RequestBody { get; set; }

        public ActionFilter()
        {
            _logger = LogManager.GetCurrentClassLogger();
        }      

        public void OnActionExecuting(ActionExecutingContext context)
        {
            watch = new Stopwatch();
            watch.Start();
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            var domains = AppSetting.GetSetting<string[]>("FrameDomains");
            if (domains != null && domains.Length > 0)
            {
                var domain_join = string.Join(" ", domains);
                context.HttpContext.Response.AddHeader("X-Frame-Options", "ALLOW-FROM " + domain_join);
                context.HttpContext.Response.AddHeader("Content-Security-Policy", "frame-ancestors " + domain_join);
            }
            watch.Stop();
            try
            {
                string res = "";
                bool success = true;
                try
                {
                    if (context.Result != null)
                    {
                        var result = (context.Result as dynamic).Value;
                        if (result != null && result?.Code != null)
                        {
                            success = result.Code == 1;
                            res = result.Data != null ? result.Data.ToString() : result.Message;
                        }
                    }                    
                }
                catch
                {                    
                    res = "未获取到结果，返回的数据无法序列化";
                    success = false;
                }

                if (AppSetting.Environment == AppEnvironment.Production)
                {
                    if (!success || watch.Elapsed.TotalMilliseconds > 5000)
                    {
                        Trace(context.HttpContext, success, res);
                    }
                    else
                    {                       
                        var controllerActionDescriptor = context.ActionDescriptor as ControllerActionDescriptor;
                        if (controllerActionDescriptor != null)
                        {
                            var Attrs = controllerActionDescriptor.MethodInfo.GetCustomAttributes(inherit: true);
                           
                            if(Attrs.Any(a => a.GetType().Equals(typeof(NoAuthorizationAttribute))))
                            {
                                //记录NoAuthorization
                                Trace(context.HttpContext, success, res);
                            }
                        }                       
                    }
                }
                else
                {
                    Trace(context.HttpContext, success, res);
                }
            }
            catch
            { }
        }

        private void Trace(HttpContext context, bool success, string res)
        {
            string url = context.GetBaseUrl() + context.Request.Path + context.Request.QueryString;
            string method = context.Request.Method;


            long contentLen = context.Request.ContentLength == null ? 0 : context.Request.ContentLength.Value;
            if (contentLen > 0)
            {
                if (context.Request.Form.Count == 0)
                {
                    // 读取请求体中所有内容
                    System.IO.Stream stream = context.Request.Body;
                    if (context.Request.Method == "POST")
                    {
                        stream.Position = 0;
                    }
                    byte[] buffer = new byte[contentLen];
                    stream.Read(buffer, 0, buffer.Length);
                    // 转化为字符串
                    RequestBody = System.Text.Encoding.UTF8.GetString(buffer);
                }
                else
                {
                    RequestBody = "";
                    foreach (var form in context.Request.Form)
                    {
                        if (RequestBody.Length > 0)
                        {
                            RequestBody += ";";
                        }
                        RequestBody += form.Key + ":" + form.Value;
                    }
                }
            }
            string info = $"\r\n地址：{url}\r\n" +
            $"方式：{method}\r\n" +
            $"Web页面：{context.Request.Headers["Referer"].ToString()}\r\n" +
            $"请求体：{RequestBody}\r\n" +
            $"结果：{res}\r\n" +
            $"客户端IP：{context.Connection.RemoteIpAddress.ToString()}\r\n" +
            //$"用户：{userID}\r\n" +                
            $"浏览器：{context.Request.GetUserAgent()}\r\n" +
            $"耗时：{watch.Elapsed.TotalMilliseconds} 毫秒";

            if (success)
            {
                _logger.Trace(info);
            }
            else
            {
                _logger.Debug(info);
            }
        }
    }

}