using Microsoft.AspNetCore.Mvc.Filters;
using System.Collections.Generic;

namespace Api.Config.Filter
{
    /// <summary>
    /// 开放访问
    /// </summary>
    public class OpenAttribute : ActionFilterAttribute
    {
        private List<string> _apps;

        public OpenAttribute()
        {
            
        }
        public OpenAttribute(params string[] app_ids)
        {
            _apps = new List<string>();
            _apps.AddRange(app_ids);
        }

        public List<string> GetApps()
        {
            return _apps;
        }
    }
}
