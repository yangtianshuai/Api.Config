using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Storage.Client;
using System;

namespace Api.Config
{
    public static class StorageExtension
    {
        public static IServiceCollection AddStorage(this IServiceCollection services, Action<StorageOptions> option)
        {
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            var options = new StorageOptions();
            option?.Invoke(options);
            //设置Service
            var service = options.GetService();
            service.Client = new ClientInfo
            {
                app_id = options.AppId,
                service_url = options.BaseUrl
            };
            options.Storage.Service = service;

            services.AddSingleton(typeof(StorageOptions), options);
            //启动存储守护者
            StorageHost.Run(options.Storage);
            return services;
        }
    }
}
