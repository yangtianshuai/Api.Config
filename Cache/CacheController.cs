using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Api.Config.Cache
{
    /// <summary>
    /// 缓存
    /// </summary>
    [Route("/cache")]
    public class CacheController : ApiCorsController
    {
        private readonly ICacheUnit _cache;
        public CacheController(ICacheUnit cache)
        {
            _cache = cache;
        }

        /// <summary>
        /// 清理缓存
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpGet("clear")]
        public async Task<IActionResult> Clear(string key)
        {
            var result = new ResponseResult();

            if(await _cache.ClearAsync(key))
            {
                result.Sucess("清理成功");
            }
            return result.ToJson();
        }
    }
}
