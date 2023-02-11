using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Api.Config.Net
{
    public static class NetExtention
    {
        /// <summary>
        /// 配置DI
        /// </summary>
        /// <param name="services">服务集合</param>
        /// <param name="name">Json的Key</param>
        /// <param name="jsonFile">配置文件</param>
        public static void SetHttp(this IServiceCollection services, Action<HttpConfig> option)
        {
            if (option != null)
            {
                var config = new HttpConfig();
                option(config);
                HttpHelper.Config = config;
            }
        }
    }
}
