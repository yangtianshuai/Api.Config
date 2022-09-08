using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Api.Config
{
    public static partial class HttpExtention
    {
        /// <summary>
        /// 快速MD5
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private static string MD5(string data)
        {
            var md5 = System.Security.Cryptography.MD5.Create();
            var dataByte = md5.ComputeHash(Encoding.UTF8.GetBytes(data));
            StringBuilder result = new StringBuilder();
            foreach (var c in dataByte)
            {
                result.Append((255 - c).ToString("X2"));
            }
            return result.ToString().ToUpper();
        }

        private static Dictionary<string, string> GetOpenSign(HttpRequest request)
        {
            var key = "OPEN-SIGN";
            if (!request.Headers.ContainsKey(key))
            {
                return null;
            }
            var open_value = request.Headers[key].ToString();
            var array = open_value.Split(',');
            var nodes = new Dictionary<string, string>();
            foreach (var node in array)
            {
                var _array = node.Split('=');
                if (_array.Length == 2)
                {
                    nodes.Add(_array[0].Trim(), _array[1].Trim());
                }
            }
            return nodes;
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
                        byte[] buffer = new byte[request.Body.Length];
                        request.Body.Read(buffer, 0, buffer.Length);
                        request.Body.Seek(0, SeekOrigin.Begin);

                        var body = Encoding.UTF8.GetString(buffer);
                        if (body.Length > 8)
                        {
                            body.Substring(0, 8);
                        }
                        dict.Add("body", body);
                    }
                    catch { }
                }
            }         

            return string.Join("&",dict.OrderBy(t => t.Key).Select(a => $"{a.Key}={ a.Value}"));

        }


        internal static bool OpenCheck(this HttpContext context, List<string> apps)
        {
            var header = GetOpenSign(context.Request);
            if (header == null || header.Count == 0)
            {
                return false;
            }
            var open_sign = DictionaryUtil.ToModel<OpenSign>(header);
            
            if (apps != null && !apps.Contains(open_sign.AppId))
            {
                return false;
            }

            if (open_sign.Timestamp != null)
            {
                //验证时间戳yyyymmddhhmmss
                var seconds = double.Parse(open_sign.Timestamp);
                seconds = seconds - DateTime.Now.Ticks / 10000000;
                if (seconds > 60 * 5)
                {
                    //验证不超过
                    return false;
                }
            }         

            var sign = BuildSign(context.Request);
            sign = MD5(sign + open_sign.Nonce);

            return sign == open_sign.Signature;
        }

    }
}
