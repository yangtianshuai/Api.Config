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
    [Route("/open")]
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
        [NoAuthorization,NoCas]
        public async Task<IActionResult> CallBackAsync(string url)
        {
            var result = new ResponseResult();
            //运维平台开放接口信息   
            var param = new HttpService().ToParam(url, null,(_param, _header) =>
            {               
                _header.SetOpenSign(_param);
            });
            var res = JsonConvert.DeserializeObject<ResponseResult>(await HttpHelper.GetStringAsync(param));
            if (res.IsSuccess())
            {
                var json = JObject.FromObject(res.Data).ToObject<OpenApiJson>();
                _openOptions.SetOpen(json);
            }
            
            return result.ToJson();
        }

        /// <summary>
        /// 刷新数据
        /// </summary>
        /// <returns></returns>
        [HttpGet("refresh")]
        public async Task<IActionResult> RefreshAsync()
        {
            var result = new ResponseResult();
            _openOptions.DownLoad(_openOptions);
            result.Sucess("刷新成功");
            return result.ToJson();
        }

    }
}
