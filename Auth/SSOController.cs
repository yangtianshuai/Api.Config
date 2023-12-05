using Microsoft.AspNetCore.Mvc;
using SSO.Client;
using System.Threading.Tasks;

namespace Api.Config.Auth
{
    /// <summary>
    /// SSO
    /// </summary>
    [Route("/sso")]
    public class SSOController : ApiCorsController
    {
        private readonly CasOptions _options;
        public SSOController(CasOptions options)
        {

        }

        [HttpGet("user/Logout")]
        public async Task<IActionResult> Logout(string ticket)
        {
            var result = new ResponseResult();  
            
            return result.ToJson();
        }
    }
}
