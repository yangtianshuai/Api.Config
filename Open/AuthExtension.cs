using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;

namespace Api.Config
{
    public static partial class HttpExtention
    {
        private static string key = "Open-Sign";
        /// <summary>
        /// 快速MD5
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private static string MD5(string data)
        {
            var md5 = System.Security.Cryptography.MD5.Create();
            var dataByte = md5.ComputeHash(Encoding.UTF8.GetBytes(data));
            var result = new StringBuilder();
            foreach (var c in dataByte)
            {
                result.Append((255 - c).ToString("X2"));
            }
            return result.ToString().ToUpper();
        }

        public static OpenSign GetOpenSign(this HttpRequest request)
        {            
            if (!request.Headers.ContainsKey(key))
            {
                return null;
            }
            var open_value = request.Headers[key].ToString();    
            if(string.IsNullOrEmpty(open_value))
            {
                return null;
            }
            return JsonConvert.DeserializeObject<OpenSign>(open_value);
        }

        private static string BuildSign(HttpRequest request)
        {
            var dict = new SortedDictionary<string, string>();

            var querys = request.Query;
            if (querys.Count > 0)
            {
                foreach (var query in querys)
                {
                    dict.Add(query.Key, query.Value);
                }
            }
            else
            {
                if (request.ContentLength != null)
                {
                    try
                    {                        
                        byte[] buffer = new byte[request.ContentLength.Value];
                        Stream stream = null;
                        request.Body.CopyTo(stream);
                        stream.Read(buffer, 0, buffer.Length);
                        // 转化为字符串                        
                        var body = Encoding.UTF8.GetString(buffer);
                        dict.Add("body", body);
                    }
                    catch { }
                }
            }         

            return string.Join("&",dict.OrderBy(t => t.Key).Select(a => $"{a.Key}={ a.Value}"));

        }

        /// <summary>
        /// 获取OpenSign
        /// </summary>
        /// <param name="context"></param>
        /// <param name="app_id"></param>
        /// <returns></returns>
        public static OpenSign GetOpenSign(this HttpContext context,string app_id = null)
        {
            var sign = new OpenSign();
            if(string.IsNullOrEmpty(app_id))
            {
                app_id = OpenOptions.AppID;
            }
            sign.AppId = app_id;            
            sign.Signature = BuildSign(context.Request);
            sign.Timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
            sign.Nonce = new Random().Next(1000,9999).ToString();
            sign.Signature = MD5(sign.Signature + sign.Nonce);
            return sign;
        }

        /// <summary>
        /// 设置OpenSign
        /// </summary>
        /// <param name="context"></param>
        /// <param name="app_id"></param>
        /// <returns></returns>
        public static void SetOpenSign(this HttpContext context, OpenSign sign = null)
        {
            if (sign == null)
            {
                sign = context.GetOpenSign();
            }
            context.Request.Headers.SetOpenSign(sign);
        }

        public static void SetOpenSign(this IHeaderDictionary headers, OpenSign sign)
        {
            headers.Add(key, GetOpenSignValue(sign));
        }

        private static string GetOpenSignValue(OpenSign sign)
        {            
            return JsonConvert.SerializeObject(sign); 
        }

        internal static void SetOpenSign(this HttpRequestHeaders headers, OpenSign sign)
        {            
            headers.TryAddWithoutValidation(key, GetOpenSignValue(sign));
        }

        internal static void SetOpenSign(this HttpContentHeaders headers, OpenSign sign)
        {
            headers.TryAddWithoutValidation(key, GetOpenSignValue(sign));
        }

        internal static bool OpenCheck(this HttpContext context, List<string> apps)
        {            
            var open_sign = GetOpenSign(context.Request); 
            if(open_sign == null)
            {
                return false;
            }
            if (apps != null && !apps.Contains(open_sign.AppId))
            {
                return false;
            }

            if (open_sign.Timestamp != null)
            {
                //验证时间戳yyyyMMddhhmmss
                var time_str = $"{ open_sign.Timestamp.Substring(0, 4)}" +
                    $"-{open_sign.Timestamp.Substring(4, 2)}" +
                    $"-{open_sign.Timestamp.Substring(6, 2)}" +
                    $" {open_sign.Timestamp.Substring(8, 2)}:{open_sign.Timestamp.Substring(10, 2)}:{open_sign.Timestamp.Substring(12, 2)}";
                var seconds = (DateTime.Now - DateTime.Parse(time_str)).TotalSeconds;                
                if (seconds > OpenOptions.OutTime)
                {
                    //验证不超过5分钟
                    return false;
                }
            }         

            var sign = BuildSign(context.Request);
            sign = MD5(sign + open_sign.Nonce);

            return sign == open_sign.Signature;
        }

    }
}
