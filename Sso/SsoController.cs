using Microsoft.AspNetCore.Mvc;
using SSO.Client;
using System.Threading.Tasks;

namespace Api.Config.Sso
{
    /// <summary>
    /// SSO
    /// </summary>
    [Route("/sso")]
    public class SsoController : ApiCorsController
    {
        private readonly SsoOptions _options;
        public SsoController(SsoOptions options)
        {
            _options = options;
        }

        [HttpGet("user/logout")]
        public async Task<IActionResult> Logout(string ticket)
        {
            return Ok();
        }

        [HttpGet("user/call_back")]
        public async Task<IActionResult> CallBack(string state)
        {
            return Ok();
        }
    }
}
