using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Api.Config.File
{
    /// <summary>
    /// 缓存
    /// </summary>
    [Route("/cache")]
    public class CacheController : ApiCorsController
    {
        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <returns></returns>
        [HttpGet("get")]
        public async Task<IActionResult> Get()
        {
            var result = new ResponseResult();
            

            return result.ToJson();
        }
    }
}
