﻿using Api.Config.Net;
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

        public static OpenApi GetApi(string id)
        {
            var api = new OpenApi();
            
            var item = OpenOptions.GetAccess(id);
            api.access_token = item.acess_token;
            return new OpenApi();
        }

        /// <summary>
        /// Get请求
        /// </summary>
        /// <param name="url">URL路径</param>  
        /// <param name="values">参数</param>  
        /// <param name="action2">返回回调</param>
        /// <returns></returns>
        public async Task<string> GetAsync(string url, Dictionary<string, string> values = null, Action<HttpResponseHeaders> action2 = null)
        {
            return await HttpHelper.GetAsync(url, values, (param, header) =>
            {
                header.SetAccessToken(access_token);
                header.SetOpenSign(param);
            }, action2);
        }
        /// <summary>
        /// Post请求
        /// </summary>
        /// <param name="url">URL路径</param>        
        /// <param name="action2">返回回调</param>
        /// <returns></returns>
        public async Task<string> PostAsync(string url, HttpContent content, Action<HttpResponseHeaders> action2 = null)
        {
            return await HttpHelper.PostAsync(url, content, (param, header) =>
            {
                header.SetOpenSign(param);
            }, action2);
        }
    }
}
