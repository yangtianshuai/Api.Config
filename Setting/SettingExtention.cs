using Api.Config.Setting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Api.Config
{
    /// <summary>
    /// Ioc扩展
    /// </summary>
    public static class SettingExtention
    {
        /// <summary>
        /// 配置DI
        /// </summary>
        /// <param name="services">服务集合</param>      
        public static IServiceCollection AddSetting(this IServiceCollection services, IConfiguration config, string evn = null)
        {
            AppSetting.Configuration = config;
            if (evn != null)
            {
                AppSetting.SetEnv(evn);
            }
            return services;
        }
        /// <summary>
        /// 配置DI
        /// </summary>
        /// <param name="services">服务集合</param>
        /// <param name="option">Setting选项</param>      
        public static void Remote(this IServiceCollection services, Action<SettingOption> option)
        {
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            if (option != null)
            {
                var settingOption = new SettingOption();
                option(settingOption);               
                AppSetting.Option = settingOption;
                AppSetting.Context = new SettingContext();

                //开启线程
                if (AppSetting.Option.DowLoad != null)
                {
                    //首次加载
                    var config = AppSetting.Option.DowLoad(AppSetting.Context);
                    AppSetting.Context.Load(config);
                    Task.Run(() =>
                    {
                        while (AppSetting.Option.PullTicks > 0)
                        {
                            Thread.Sleep(AppSetting.Option.PullTicks * 1000);
                            //拉取
                            config = AppSetting.Option.DowLoad(AppSetting.Context);
                            AppSetting.Context.Load(config);
                        }
                    });
                }

            }
        }
    }
}