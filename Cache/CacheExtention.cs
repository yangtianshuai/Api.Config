using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Api.Config.Cache
{
    public static class CacheExtention
    {
        /// <summary>
        /// 添加内存型Session
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddMemoryCache(this IServiceCollection services)
        {
            services.AddMemoryCache();
            services.AddSingleton<ICacheUnit, MemoryCacheUnit>();            
            services.AddSingleton<CacheUnit>();
            return services;
        }
    }
}
