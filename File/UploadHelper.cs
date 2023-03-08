using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Storage.Client;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Api.Config
{
    public class UploadHelper
    { 
        /// <summary>
        /// 存储代理上传
        /// </summary>        
        /// <param name="storage"></param>
        /// <param name="stream">文件流</param>
        /// <param name="file_url">本地URL</param>
        /// <param name="file_name">文件名</param>
        /// <param name="file_ext"></param>
        /// <returns></returns>
        public static async Task<PreUploadView> UploadProxy(AbstractStorage storage, FileStream stream
           , string file_url, string file_name, string file_ext)
        {
            byte[] tempbuffer = new byte[1];
            //var count2= await stream.ReadAsync(tempbuffer, 0, 1);
            //存储控制-保存记录
            var submit = new PreStoreSubmit
            {
                size = stream.Length,
                name = file_name,
                file_ext = file_ext.Replace(".",""),
                url = file_url,
                file_type = storage.GetMode()
            };            
            submit.md5 = GetMD5(stream); //文件指纹 
            stream.Position = 0;
            var upload_view = await storage.Service.UploadAsync(submit);
            if (upload_view != null && upload_view.configs != null)
            {
                upload_view.configs.Sort();//按照slice_no排序
                foreach (var config in upload_view.configs)
                {
                    var fs = new FileSegment
                    {
                        file_id = upload_view.file_id,
                        file_no = config.slice_no
                    };
                    fs.content = new byte[config.size];
                    //var srcArray = System.IO.File.ReadAllBytes(stream.Name);
                    //Buffer.BlockCopy(srcArray, 0, fs.content, 0, srcArray.Length);                   
                    await stream.ReadAsync(fs.content, 0, (int)config.size);                    
                    //文件分片，写入本地
                    StorageHost.AddTask(fs, config);
                }
            }  
            return upload_view;
        }

        public static string GetMD5(FileStream stream)
        {            
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] retVal = md5.ComputeHash(stream);           
            md5.Clear();
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < retVal.Length; i++)
            {
                sb.Append(retVal[i].ToString("x2"));
            }
            return sb.ToString();  
        }

        public static string GetUrl(HttpContext context, string file_path)
        {
            var file_url = context.GetBaseUrl() + file_path.Replace(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"), "");
            file_url = file_url.Replace("\\", "/");
            return file_url;
        }

        public static string GetPath(HttpContext context, string url)
        {
            var directory = Directory.GetCurrentDirectory();
            directory = Path.Combine(directory, "wwwroot");
            if (!Directory.Exists(directory))
            {
                return "";
            }
            var file_path = directory + url.Replace(context.GetBaseUrl(), "");
            return file_path;
        }

        public static string GetDirectory(bool www_flag = true)
        {
            var directory = Directory.GetCurrentDirectory();
            directory = Path.Combine(directory, "wwwroot");
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            if (!www_flag)
            {
                directory = Directory.GetCurrentDirectory();
            }
            directory = Path.Combine(directory, "storage");
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            directory = Path.Combine(directory, DateTime.Now.Year.ToString());
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            directory = Path.Combine(directory, DateTime.Now.Month.ToString());
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            directory = Path.Combine(directory, DateTime.Now.Day.ToString());
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            return directory;
        }
    }
}
