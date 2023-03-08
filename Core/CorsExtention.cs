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
            string[] origins;
            if (string.IsNullOrEmpty(allowOrigins))
            {
                origins = AppSetting.GetSetting<string[]>("AllowOrigins");                
            }            
            else
            {
                origins = allowOrigins.Split(',');
            }
            if (origins == null || origins.Length == 0)
            {
                origins = new string[] { "*" };
            }


            var allowMethods = AppSetting.GetSetting("AllowMethods");
            string[] methods;
            if (string.IsNullOrEmpty(allowMethods))
            {
                methods = AppSetting.GetSetting<string[]>("AllowMethods");                
            }
            else
            {
                methods = allowMethods.Split(',');  
            }
            if (methods == null || methods.Length == 0)
            {
                methods = new string[] { "*" };
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