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
    public class UploadMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly StorageOptions _storageOptions;

        public UploadMiddleware(RequestDelegate next, StorageOptions storageOptions)
        {
            _next = next;
            _storageOptions = storageOptions;
        }

        public async Task Invoke(HttpContext context)
        {
            if (!context.Request.HasFormContentType)
            {
                await _next.Invoke(context);
                return;
            }
            
            var path = context.Request.Path.Value.TrimEnd('/');
       
            if (_storageOptions.Storage.GetUrl() == path)
            {
                await UploadAsync(context);
            }
            else if (_storageOptions.Storage.GetSliceUrl() == path)
            {
                await UploadSliceAsync(context);
            }
            await _next.Invoke(context);
        }


        private async Task UploadAsync(HttpContext context)
        {
            try
            {
                var result = new ResponseResult();
                
                var directory = GetDirectory();

                //保存本地文件
                foreach (var file in context.Request.Form.Files)
                {
                    var file_name = Guid.NewGuid().ToString("N");
                    var file_ext = Path.GetExtension(file.FileName);
                    var file_path = Path.Combine(directory, file_name + file_ext);
                    using (var stream = new FileStream(file_path, FileMode.Create))
                    {
                        //保存本地文件
                        await file.CopyToAsync(stream);                        
                    }
                    using (var stream = new FileStream(file_path, FileMode.Open, FileAccess.Read))
                    {
                        //上传文件
                        var upload_view = await UploadProxy(context, _storageOptions.Storage, stream, file_path, file.FileName, file_ext);

                        result.Data = new
                        {
                            upload_view.file_id,
                            url = upload_view.mirror_id
                        };
                    }                    
                    context.Response.StatusCode = (int)HttpStatusCode.OK;
                    context.Response.ContentType = "application/json;charset=utf-8";
                    await context.Response.WriteAsync(JsonConvert.SerializeObject(result));
                }
            }
            catch (Exception ex)
            {
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                //向前端返回
                await context.Response.WriteAsync("文件上传发生错误：" + ex.StackTrace);
            }
        }

        private async Task UploadSliceAsync(HttpContext context)
        {
            try
            {
                var result = new ResponseResult();

                var file = context.Request.Form.Files[0];
                var file_id = context.Request.Headers["file_id"].ToString();
                //保存本地文件
                var file_count = int.Parse(context.Request.Headers["file_count"].ToString());
                var file_no = int.Parse(context.Request.Headers["file_no"].ToString());
                var file_ext = context.Request.Headers["file_ext"].ToString();

                var directory = GetDirectory();

                directory = Path.Combine(directory, context.GetRemoteIp().Replace(":", "") + file_id);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                var file_path = Path.Combine(directory, file_no + file_ext);
                using (var stream = new FileStream(file_path, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                var files = Directory.GetFiles(directory);
                if (file_count == files.Length)
                {
                    var file_name = Guid.NewGuid().ToString("N");
                    directory = GetDirectory();
                    file_path = Path.Combine(directory, file_name + file_ext);
                    using (var stream = new FileStream(file_path, FileMode.Create))
                    {
                        foreach (var sclice_file in files.OrderBy(x => x.Length).ThenBy(x => x))
                        {
                            var bytes = File.ReadAllBytes(sclice_file);
                            await stream.WriteAsync(bytes, 0, bytes.Length);
                            bytes = null;
                        }                        
                    }
                    using (var stream = new FileStream(file_path, FileMode.Open, FileAccess.Read))
                    {
                        await UploadProxy(context, _storageOptions.Storage, stream, file_path, file.FileName, file_ext);
                    }
                    Directory.Delete(directory);//删除临时文件                    
                }
                else
                {
                    result.Sucess("上传成功");
                }
                context.Response.StatusCode = (int)HttpStatusCode.OK;
                context.Response.ContentType = "application/json;charset=utf-8";
                await context.Response.WriteAsync(JsonConvert.SerializeObject(result));
            }
            catch (Exception ex)
            {
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                //向前端返回
                await context.Response.WriteAsync("文件上传发生错误：" + ex.StackTrace);
            }
        }

        private async Task<PreUploadView> UploadProxy(HttpContext context, AbstractStorage storage, FileStream stream
           , string file_path, string file_name, string file_ext)
        {

            var file_url = context.GetBaseUrl() + file_path.Replace(Directory.GetCurrentDirectory(), "");
            file_url = file_url.Replace("\\","/");
            //存储控制-保存记录
            var submit = new PreStoreSubmit
            {
                size = stream.Length,
                name = file_name,
                file_ext = file_ext.Replace(".",""),
                url = file_url,
                file_type = storage.GetMode(),
                md5 = GetMD5(stream) //文件指纹
            };
            var upload_view = await storage.Service.UploadAsync(submit);
            if (upload_view.configs != null)
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
                    await stream.ReadAsync(fs.content, 0, (int)config.size);
                    //文件分片，写入本地
                    StorageHost.AddTask(fs, config);
                }
            }        
            return upload_view;
        }

        private string GetMD5(FileStream stream)
        {            
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] retVal = md5.ComputeHash(stream);         
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < retVal.Length; i++)
            {
                sb.Append(retVal[i].ToString("x2"));
            }
            return sb.ToString();  
        }

        private string GetDirectory()
        {
            var directory = Directory.GetCurrentDirectory();
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
