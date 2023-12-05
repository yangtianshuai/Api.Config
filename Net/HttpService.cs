using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Api.Config.Net
{
    public class HttpService
    {
        public HttpParam ToParam(string url, Dictionary<string, string> values = null, Action<HttpParam, HttpRequestHeaders> action = null, Action<HttpResponseHeaders> action2 = null)
        {
            return new HttpParam
            {
                url = url,
                values = values,
                http_request_action = action,
                responese_action = action2,
                sucess_ensure = HttpHelper.Config.SuccessOnly,
                timeout = HttpHelper.Config.TimeOut
            };
        }

        public HttpParam ToParam(string url, Dictionary<string, string> values = null, Action<HttpRequestHeaders> action = null, Action<HttpResponseHeaders> action2 = null)
        {
            return new HttpParam
            {
                url = url,
                values = values,
                request_action = action,
                responese_action = action2,
                sucess_ensure = HttpHelper.Config.SuccessOnly,
                timeout = HttpHelper.Config.TimeOut
            };
        }

        public HttpParam ToParam2(string url, HttpContent content, Action<HttpRequestHeaders> action = null, Action<HttpResponseHeaders> action2 = null)
        {
            return new HttpParam
            {
                url = url,
                request_content = content,
                request_action = action,
                responese_action = action2,
                sucess_ensure = HttpHelper.Config.SuccessOnly,
                timeout = HttpHelper.Config.TimeOut
            };
        }

        public HttpParam ToParam2(string url, HttpContent content, Action<HttpParam, HttpRequestHeaders> action = null, Action<HttpResponseHeaders> action2 = null)
        {
            return new HttpParam
            {
                url = url,
                request_content = content,
                http_request_action = action,
                responese_action = action2,
                sucess_ensure = HttpHelper.Config.SuccessOnly,
                timeout = HttpHelper.Config.TimeOut
            };
        }
    }
}
