using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Api.Config.Log
{
    /// <summary>
    /// 日志
    /// </summary>
    [Route("log")]
    public class LogController : ApiCorsController
    {
        private string dir_path = "logs";
        private readonly IHostingEnvironment _hostingEnvironment;

        public LogController(IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }

        /// <summary>
        /// 获取文件
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetFiles")]
        public async Task<IActionResult> GetFiles()
        {
            var result = new ResponseResult();              
            var dirctory = Path.Combine(Directory.GetCurrentDirectory(), dir_path);

            var list = new List<string>();
            string[] files = Directory.GetFiles(dirctory);

            foreach (var file in files)
            {
                list.Add(file.Replace(dirctory, ""));
            }
            result.Data = list;
            return result.ToJson();
        }
        /// <summary>
        /// 下载
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        [HttpGet("Download")]
        public async Task<IActionResult> Download(string path)
        {
            var result = new ResponseResult();
            path = Path.Combine(Directory.GetCurrentDirectory(), dir_path, path);
            using (FileStream fs = new FileStream(path, FileMode.Open))
            {
                using (StreamReader reader = new StreamReader(fs))
                {
                    string content = await reader.ReadToEndAsync();
                    content = new Regex("\r\n").Replace(content, "<br>");
                    result.Data = content;
                }                
            }
            return result.ToJson();
        }
    }
}
