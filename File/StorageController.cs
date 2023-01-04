using Microsoft.AspNetCore.Mvc;
using Storage.Client;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api.Config.File
{
    /// <summary>
    /// 存储
    /// </summary>
    [Route("storage")]
    public class StorageController : ApiCorsController
    {
        private readonly StorageOptions _storageOptions;

        public StorageController(StorageOptions storageOptions)
        {
            _storageOptions = storageOptions;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadAsync()
        {
            var result = new ResponseResult();

            var directory = UploadHelper.GetDirectory(true);

            var list = new List<object>();

            //保存本地文件
            foreach (var file in HttpContext.Request.Form.Files)
            {
                var file_name = Guid.NewGuid().ToString("N");
                var file_ext = Path.GetExtension(file.FileName);
                var file_path = Path.Combine(directory, file_name + file_ext);
                using (var stream = new FileStream(file_path, FileMode.Create))
                {
                    //保存本地文件
                    await file.CopyToAsync(stream);
                }
                var file_url = UploadHelper.GetUrl(HttpContext, file_path);
                using (var stream = new FileStream(file_path, FileMode.Open, FileAccess.Read))
                {
                    //上传文件
                    var upload_view = await UploadHelper.UploadProxy( _storageOptions.Storage, stream, file_url, file.FileName, file_ext);

                    list.Add(new
                    {
                        file_name,
                        url = file_url,
                        upload_view?.file_id                       
                    });
                }                
            }
            result.Data = list;
            return result.ToJson();
        }

        [HttpPost("upload/slice")]
        public async Task<IActionResult> UploadSliceAsync()
        {
            var result = new ResponseResult();

            var file = HttpContext.Request.Form.Files[0];
            var file_id = HttpContext.Request.Form["file_id"].ToString();
            //保存本地文件
            var file_count = int.Parse(HttpContext.Request.Form["file_count"].ToString());
            //分片序号
            var file_no = int.Parse(HttpContext.Request.Form["file_no"].ToString());
            var file_name = HttpContext.Request.Form["file_name"].ToString();
            var file_ext = "";
            var array = file_name.Split('.');
            if (array.Length > 1)
            {
                file_ext = array[array.Length-1];
            }            

            var directory = UploadHelper.GetDirectory();

            directory = Path.Combine(directory, HttpContext.GetRemoteIp().Replace(":", "") + file_id);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            var file_path = Path.Combine(directory, file_no + (file_ext.Length > 0 ? "." + file_ext : ""));
            using (var stream = new FileStream(file_path, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var files = Directory.GetFiles(directory);
            if (file_count == files.Length)
            {
                var new_file_name = Guid.NewGuid().ToString("N");               
                file_path = Path.Combine(UploadHelper.GetDirectory(true), new_file_name + (file_ext.Length > 0 ? "." + file_ext : ""));
                using (var stream = new FileStream(file_path, FileMode.Create))
                {
                    foreach (var sclice_file in files.OrderBy(x => x.Length).ThenBy(x => x))
                    {
                        var bytes = System.IO.File.ReadAllBytes(sclice_file);
                        await stream.WriteAsync(bytes, 0, bytes.Length);
                        bytes = null;
                    }
                }

                var file_url = UploadHelper.GetUrl(HttpContext, file_path);

                using (var stream = new FileStream(file_path, FileMode.Open, FileAccess.Read))
                {
                    var upload_view = await UploadHelper.UploadProxy(_storageOptions.Storage, stream, file_url, file_name, file_ext);
                    result.Data = new
                    {
                        file_name,
                        url = file_url,
                        upload_view?.file_id
                    };
                }
                new DirectoryInfo(directory).Delete(true);//删除临时文件
            }
            else
            {
                result.Sucess("上传成功");
            }            
            return result.ToJson();
        }

        [HttpGet("download")]
        public async Task<IActionResult> DownloadAsync()
        {
            var result = new ResponseResult();
            var file_id = HttpContext.Request.Query["id"].ToString();
            var file_name = HttpContext.Request.Query["name"].ToString();

            if (!string.IsNullOrEmpty(file_id))
            {
                //从存储平台获取数据

                //_storageOptions.Storage

            }
            return result.ToJson();
        }


    }
}
