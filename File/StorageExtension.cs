﻿using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Storage.Client;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Api.Config
{
    public static class StorageExtension
    {
        public static IServiceCollection AddStorage(this IServiceCollection services, Action<StorageOptions> option)
        {
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            var options = new StorageOptions();
            if (option != null)
            {
                option(options);
            }
            //设置Service
            var service = options.GetService();
            service.Client = new ClientInfo
            {
                app_id = options.AppId,
                service_url = options.BaseUrl
            };
            options.Storage.Service = service;

            services.AddSingleton(typeof(StorageOptions), options);

            ////普通上传URL
            //MvcRouter.Add(options.Storage.GetUrl(), async context =>
            //{
            //    await UploadAsync(context, options.Storage);
            //});
            ////分片上传URL
            //MvcRouter.Add(options.Storage.GetSliceUrl(), async context =>
            //{
            //    await UploadSliceAsync(context, options.Storage);
            //});
            //启动存储守护者
            StorageHost.Run(options.Storage);
            return services;
        }

        public static void UseStorage(this IApplicationBuilder builder)
        {
            builder.UseMiddleware<UploadMiddleware>();
        }
    }
}
