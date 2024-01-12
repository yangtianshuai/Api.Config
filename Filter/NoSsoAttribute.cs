using Microsoft.AspNetCore.Mvc.Filters;

namespace Api.Config
{
    /// <summary>
    /// 不需要SSO过滤
    /// </summary>
    public class NoSsoAttribute : ActionFilterAttribute
    {        
        
    }
}
