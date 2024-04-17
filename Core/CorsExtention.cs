using Api.Config.Setting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

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
        [Obsolete("已过期的旧方法，推荐使用AddHttpCors")]
        public static void AddCors2(this IServiceCollection services)
        {
            var item = new CorsItem();
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
            item.Origins = new List<string>(origins);


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

            item.Methods = new List<string>(methods);
            item.PolicyName = "any";
            //添加跨域解决方案           
            AddHttpCors(services, config =>
            {
                config.Add(item);
            });
        }

        /// <summary>
        /// 添加Cors跨域方案
        /// 开启安全验证时，需要配置AllowOrigins
        /// 允许配置AllowMethods
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static void AddHttpCors(this IServiceCollection services, Action<CorsConfig> action = null)
        {            
            if (action == null)
            {
                //添加跨域解决方案
                services.AddCors(options =>
                {
                    options.AddPolicy("any", builder =>
                    {
                        builder.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials();
                    });
                });
            }
            else
            {
                var config = new CorsConfig();
                action(config);

                services.AddCors(options =>
                {
                    foreach(var item in config.Items())
                    {
                        options.AddPolicy(item.PolicyName, builder =>
                        {
                            if(item.Origins == null || item.Origins.Count == 0)
                            {
                                builder.AllowAnyOrigin();
                            }
                            else
                            {
                                builder.WithOrigins(item.Origins.ToArray());
                            }

                            if (item.Methods == null || item.Methods.Count == 0)
                            {
                                builder.AllowAnyMethod();
                            }
                            else
                            {
                                builder.WithOrigins(item.Methods.ToArray());
                            }
                            if (item.Headers == null || item.Headers.Count == 0)
                            {
                                builder.AllowAnyHeader();
                            }
                            else
                            {
                                builder.WithHeaders(item.Headers.ToArray());
                            }

                            builder.AllowCredentials();
                        });
                    }                    
                });
            }
        }
    }

    public class CorsConfig
    {
        private List<CorsItem> items;
        public CorsConfig()
        {
            items = new List<CorsItem>();
        }

        public void Add(CorsItem item)
        {
            items.Add(item);
        }

        internal List<CorsItem> Items()
        {
            return items;
        }
    }

    public class CorsItem
    {
        public string PolicyName { get; set; }
        public List<string> Origins { get; set; }
        public List<string> Methods { get; set; }
        public List<string> Headers { get; set; }
    }
}