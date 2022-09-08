using Api.Config.Setting;
using Microsoft.Extensions.DependencyInjection;

namespace Api.Config
{
    public static class CorsExtention
    {
        /// <summary>
        /// 添加Cors跨域方案
        /// 开启安全验证时，需要配置AllowOrigins
        /// 允许配置AllowMethods
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddCors2(this IServiceCollection services)
        {
            var allowOrigins = AppSetting.GetSetting("AllowOrigins");
            var origins = new string[] { "*" };
            if (allowOrigins != null)
            {
                origins = allowOrigins.Split(',');
            }

            var allowMethods = AppSetting.GetSetting("AllowMethods");
            var methods = new string[] { "*" };
            if (allowMethods != null)
            {
                methods = allowMethods.Split(',');
            }
            //添加跨域解决方案
            services.AddCors(options =>
            {
                options.AddPolicy("any", builder =>
                {
                    builder.WithOrigins(origins)
                    .WithMethods(methods)
                    .AllowAnyHeader()
                    .AllowCredentials();//指定处理cookie
                });
            });
            return services;
        }         
    }
}