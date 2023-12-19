using Api.Config.Net;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
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
                options.DownLoad = action;
                action(options);
            }
            if(!string.IsNullOrEmpty(OpenOptions.AppID))
            {
                services.AddSingleton(options);
            }
            //开启线程
            if (options.DownLoad != null && !string.IsNullOrEmpty(OpenOptions.AppID))
            {                
                Task.Run(() =>
                {
                    int counter = 1;
                    while (OpenOptions.OutTime > 0)
                    {
                        if (counter > 10)
                        {
                            counter = 1;
                        }                        
                        try
                        {
                            //拉取
                            options.DownLoad(options);
                            counter = 1;
                        }
                        catch
                        {
                            counter++;
                        }
                        Thread.Sleep(OpenOptions.OutTime * counter * 1000);
                    }                 
                });
            }
            //加载权限
            return services;
        }      

        internal static OpenSign GetOpenSign(this HttpParam param, string app_id = null)
        {
            var open_sign = new OpenSign();

            var dict = new Dictionary<string, string>();
            if (param.request_content != null)
            {
                byte[] buffer = param.request_content.ReadAsByteArrayAsync().GetAwaiter().GetResult();

                int count = OpenSign.MaxBodyLen();
                if (buffer.Length < count)
                {
                    count = buffer.Length;
                }
                byte[] buffer2 = new byte[count];
                Array.Copy(buffer,0, buffer2, 0, count);

                // 转化为字符串                        
                var body = Encoding.UTF8.GetString(buffer);
                dict.Add(OpenSign.BodyKey(), body);               
            }
            else
            {
                dict = param.url.GetQuery();
            }
            var sign = open_sign.Get(dict);
            if (string.IsNullOrEmpty(app_id))
            {
                app_id = OpenOptions.AppID;
            }
            sign.AppId = app_id;

            return sign;
        }

        

    }
}
