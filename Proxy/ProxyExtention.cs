using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Api.Config.Proxy
{
    public static class ProxyExtention
    {
        /// <summary>
        /// 添加方向代理
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddProxy(this IServiceCollection services)
        {
            //hosts:[{host:'xxx.com',rate:20,routes:[]}]
            //routes:['','']或者*(表示全部)
            //except:['','']除了某些外
            var json = Path.Combine(AppContext.BaseDirectory, "proxy.json");
            var jsonConfiguration = System.IO.File.ReadAllText(json);
            var setting = JsonConvert.DeserializeObject<ProxySetting>(jsonConfiguration);
            var options = new ProxyOptions();
            options.Set(setting);            
            services.AddSingleton(typeof(ProxyOptions), options);
            return services;
        }

        /// <summary>
        /// 添加反向代理
        /// </summary>
        /// <param name="services"></param>
        /// <param name="option"></param>
        /// <returns></returns>
        public static IServiceCollection AddProxy(this IServiceCollection services, Action<ProxyOptions> option)
        {
            var options = new ProxyOptions();
            options.Set(new ProxySetting());
            if (option != null)
            {
                option(options);                
            }
            services.AddSingleton(typeof(ProxyOptions), options);
            return services;
        }

    }
}
