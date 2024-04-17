using Microsoft.AspNetCore.Mvc;

namespace Api.Config.Response
{
    public interface INoAuthorizationResult
    {
        ActionResult Result(ActionResult result);
    }
}
