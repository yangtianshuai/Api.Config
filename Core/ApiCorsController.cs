using Microsoft.AspNetCore.Cors;

namespace Api.Config
{
    [EnableCors("any")]
    public class ApiCorsController : ApiJsonController
    {

    }
}