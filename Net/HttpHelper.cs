using NLog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Api.Config.Net
{
    public class HttpHelper
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        internal static HttpConfig Config { get; set; } = new HttpConfig();        

        /// <summary>
        /// Get请求
        /// </summary>
        /// <param name="url">URL路径</param>
        /// <param name="values">参数集合</param>
        /// <param name="action">请求回调</param>
        /// <param name="action2">返回回调</param>
        /// <returns></returns>
        public static async Task<string> GetAsync(string url, Dictionary<string, string> values = null, Action<HttpRequestHeaders> action = null, Action<HttpResponseHeaders> action2 = null)
        {         
            return await GetStringAsync(new HttpService().ToParam(url, values, action, action2));
        }

        /// <summary>
        /// Get请求,返回字符串
        /// </summary>
        /// <param name="param">请求参数</param>
        /// <returns></returns>
        public static async Task<string> GetStringAsync(HttpParam param)
        {
            var _content = await GetAsync(param);
            var body = await _content.ReadAsStringAsync();
            return body;
        }

        /// <summary>
        /// Get请求
        /// </summary> 
        /// <param name="param">请求参数</param>
        /// <returns></returns>       
        public static async Task<HttpContent> GetAsync(HttpParam param)
        {
            param.url = param.GetUri();


            try
            {
                using (var client = new HttpClient(GetHandler(param.url)))
                {
                    client.Timeout = TimeSpan.FromSeconds(param.timeout);

                    //设置代理
                    param.url = SetProxy(param.url);
                    param.request_action?.Invoke(client.DefaultRequestHeaders);
                    param.http_request_action?.Invoke(param, client.DefaultRequestHeaders);

                    Stopwatch watch = new Stopwatch();
                    watch.Start();

                    var response = await client.GetAsync(param.url);
                    watch.Stop();
                    if (watch.Elapsed.TotalMilliseconds > 5000)
                    {
                        _logger.Error($"HTTP发起GET请求耗时{watch.Elapsed.TotalMilliseconds}ms，URL：{param.url}");
                    }
                    if (response != null)
                    {
                        if (param.sucess_ensure)
                        {
                            response.EnsureSuccessStatusCode();
                        }

                        param.responese_action?.Invoke(response.Headers);

                        return response.Content;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"HTTP发起GET请求发生错误：{ex.Message}，URL：{param.url}");
            }
            return null;
        }

        private static string SetProxy(string url)
        {
            var uri = new Uri(url);

            var configs = Config.Proxys.Where(t => t.Value.Contains(uri.Host));
            //全局注入[{ new_host: 数组}]
            if (configs.Count() > 0)
            {
                url = url.Replace(uri.Host, configs.ToList()[0].Key);
            }
            return url;
        }                

        /// <summary>
        /// Post请求
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <param name="content">参数</param>
        /// <param name="action">请求回调</param>
        /// <param name="action2">返回回调</param>
        /// <returns></returns>
        public static async Task<string> PostAsync(string url, HttpContent content, Action<HttpRequestHeaders> action = null, Action<HttpResponseHeaders> action2 = null)
        {
            return await PostStringAsync(new HttpService().ToParam2(url, content, action, action2));
        }

        /// <summary>
        /// Post请求,返回字符串
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static async Task<string> PostStringAsync(HttpParam param)
        {
            var _content = await PostAsync(param);
            var body = await _content.ReadAsStringAsync();
            return body;
        }

        /// <summary>
        /// Post请求
        /// </summary>       
        /// <param name="param">Http内容</param>
        /// <returns></returns>
        public static async Task<HttpContent> PostAsync(HttpParam param)
        {
            try
            {               
                using (var client = new HttpClient(GetHandler(param.url)))
                {
                    client.Timeout = TimeSpan.FromSeconds(param.timeout);

                    //设置代理
                    param.url = SetProxy(param.url);

                    param.request_action?.Invoke(client.DefaultRequestHeaders);
                    param.http_request_action?.Invoke(param,client.DefaultRequestHeaders);

                    Stopwatch watch = new Stopwatch();
                    watch.Start();

                    var response = await client.PostAsync(param.url, param.request_content);
                    watch.Stop();

                    if (watch.Elapsed.TotalMilliseconds > 5000)
                    {
                        _logger.Error($"HTTP发起POST请求耗时{watch.Elapsed.TotalMilliseconds}ms，URL：{param.url}");
                    }
                    if (response != null)
                    {
                        if (param.sucess_ensure)
                        {
                            response.EnsureSuccessStatusCode();
                        }
                        param.responese_action?.Invoke(response.Headers);

                        return response.Content;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"HTTP发起POST请求发生错误：{ex.Message}，URL：{param.url}");
            }

            return null;
        }


        private static HttpClientHandler GetHandler(string url)
        {
            var handler = new HttpClientHandler();
            if (url.StartsWith("https://"))
            {
                handler.ClientCertificateOptions = ClientCertificateOption.Manual;
                handler.ServerCertificateCustomValidationCallback = (se, cert, chain, sslerror) => { return true; };
            }
            return handler;
        }
    }

    public class HttpParam
    {
        /// <summary>
        /// 请求URL
        /// </summary>
        public string url { get; set; }
        public Dictionary<string, string> values { get; set; }
        public HttpContent request_content { get; set; }
        public Action<HttpParam, HttpRequestHeaders> http_request_action { get; set; }
        public Action<HttpRequestHeaders> request_action { get; set; }
        public Action<HttpResponseHeaders> responese_action { get; set; }
        public bool sucess_ensure { get; set; }
        public int timeout { get; set; }

        public string GetUri()
        {
            if (this.values != null)
            {
                string query = "";
                foreach (var value in this.values)
                {
                    if (query.Length > 0)
                    {
                        query += "&";
                    }
                    query += $"{ value.Key}={ value.Value}";
                }
                if (query.Length > 0)
                {
                    return this.url + "?" + query;
                }
            }
            return this.url;
        }
    }
}
