using Api.Config.Net;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
                
        [HttpGet("call_back")]
        public async Task<IActionResult> CallBackAsync(string url)
        {
            var result = new ResponseResult();
            //运维平台开放接口信息            
            var res = JsonConvert.DeserializeObject<ResponseResult>(await HttpHelper.GetAsync(url, (param, header) =>
            {                
                header.SetOpenSign(param);
            }));
            if (res.IsSuccess())
            {
                var json = JObject.FromObject(res.Data).ToObject<OpenApiJson>();
                _openOptions.SetOpen(json);
            }
            
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
