using Api.Config.Net;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Api.Config.Open
{
    public class OpenApi
    {
        private OpenApi()
        {

        }

        private string access_token;
        private string url;

        public static OpenApi Get(string id = null)
        {
            var api = new OpenApi();
            if(!string.IsNullOrEmpty(id))
            {
                var item = OpenOptions.GetAccess(id);
                if (item == null)
                {
                    throw new Exception("未发现开发接口");
                }
                api.access_token = item.acess_token;
                api.url = item.path;
            }           

            return new OpenApi();
        }

        public OpenApi SetUrl(string url)
        {
            this.url = url;
            return this;
        }

        /// <summary>
        /// Get请求
        /// </summary>        
        /// <param name="values">参数</param>  
        /// <param name="action2">返回回调</param>
        /// <returns></returns>
        public async Task<string> GetAsync(Dictionary<string, string> values = null, Action<HttpResponseHeaders> action2 = null)
        {
            if(string.IsNullOrEmpty(url))
            {
                throw new Exception("URL不能为空");
            }
            var param = new HttpService().ToParam(url, values, (_param, _header) =>
            {
                if (!string.IsNullOrEmpty(access_token))
                {
                    _header.SetAccessToken(access_token);
                }
                _header.SetOpenSign(_param);
            }, action2);
            return await HttpHelper.GetStringAsync(param);
        }
        
        /// <summary>
        /// Post请求
        /// </summary>       
        /// <param name="action2">返回回调</param>
        /// <returns></returns>
        public async Task<string> PostAsync(HttpContent content, Action<HttpResponseHeaders> action2 = null)
        {
            if (string.IsNullOrEmpty(url))
            {
                throw new Exception("URL不能为空");
            }
            var param = new HttpService().ToParam2(url, content, (_param, _header) =>
            {
                if (!string.IsNullOrEmpty(access_token))
                {
                    _header.SetAccessToken(access_token);
                }
                _header.SetOpenSign(_param);
            }, action2);
            return await HttpHelper.GetStringAsync(param);
        }
    }
}
