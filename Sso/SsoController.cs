using Microsoft.AspNetCore.Mvc;
using SSO.Client;
using System.Net;
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

        [HttpGet("user/logout"),NoAuthorization]
        public async Task<IActionResult> Logout(string ticket)
        {
            return Ok("success");
        }

        [HttpGet("user/call_back"), NoAuthorization]
        public async Task<IActionResult> CallBack(string state)
        {
            if(HttpContext.Response.StatusCode == (int)HttpStatusCode.Redirect)
            {
                var url = HttpContext.Response.Headers["redirect-url"].ToString();
                return new RedirectResult(url, true);
            }
            
            return Ok();
        }
    }
}
