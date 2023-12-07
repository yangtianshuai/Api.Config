using Microsoft.Extensions.DependencyInjection;

namespace Api.Config.Cache
{
    public static class CacheExtention
    {
        /// <summary>
        /// 添加内存型Session
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddCache(this IServiceCollection services)
        {
            services.AddMemoryCache();
            services.AddSingleton<ICacheUnit, MemoryCacheUnit>();
            SetCache(services);
            return services;
        }

        /// <summary>
        /// 添加内存型Session
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddRedisCache(this IServiceCollection services)
        {            
            services.AddSingleton<ICacheUnit, RedisCacheUnit>();
            SetCache(services);

            return services;
        }

        private static void SetCache(this IServiceCollection services)
        {
            var cache = services.BuildServiceProvider().GetService<ICacheUnit>();
            var cache_unit = new CacheUnit(cache);
            services.AddSingleton(typeof(CacheUnit), cache_unit);
        }
    }
}
