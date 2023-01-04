using Microsoft.AspNetCore.Mvc;
using Storage.Client;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api.Config.Open
{
    /// <summary>
    /// 开放接口
    /// </summary>
    [Route("open")]
    public class OpenController : ApiCorsController
    {
        private readonly OpenOptions _openOptions;

        public OpenController(OpenOptions openOptions)
        {
            _openOptions = openOptions;
        }

        [HttpGet("get")]
        public IActionResult GetApps()
        {
            var result = new ResponseResult();
            result.Data = _openOptions.GetApps();
            return result.ToJson();
        }

        [HttpGet("refresh")]
        public async Task<IActionResult> RefreshAppsAsync()
        {
            var result = new ResponseResult();
            _openOptions.Clear();
            _openOptions.Action(_openOptions);
            result.Sucess("刷新成功");
            return result.ToJson();
        }

    }
}
