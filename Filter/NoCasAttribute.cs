using Microsoft.AspNetCore.Mvc.Filters;

namespace Api.Config
{
    /// <summary>
    /// 不需要Cas过滤
    /// </summary>
    public class NoCasAttribute : ActionFilterAttribute
    {        
        
    }
}
