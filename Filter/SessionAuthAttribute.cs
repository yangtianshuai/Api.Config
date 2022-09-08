using Microsoft.AspNetCore.Mvc.Filters;

namespace Api.Config
{
    /// <summary>
    /// 白名单下需要Session验证
    /// </summary>
    public class SessionAuthAttribute : ActionFilterAttribute
    {
        
    }
}
