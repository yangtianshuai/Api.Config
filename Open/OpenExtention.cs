using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Api.Config
{
    public static class OpenExtention
    {
        /// <summary>
        /// 添加Open开放验证
        /// </summary>
        /// <param name="services"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static IServiceCollection AddOpen(this IServiceCollection services,Action<OpenOptions> action)
        {
            var options = new OpenOptions();
            if (action != null)
            {
                options.Action = action;
                action(options);
            }
            services.AddSingleton(options);
            //加载权限
            return services;
        }        

    }
}
