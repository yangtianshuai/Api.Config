using Api.Config.Core;
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

        public List<string> GetApps(string path = null)
        {            
            if (_apps != null && _apps.Count > 0)
            {
                return _apps;
            }
            if(!string.IsNullOrEmpty(path) && OpenOptions.OpenApps.ContainsKey(path))
            {
                return OpenOptions.OpenApps[path];
            }
            //第一次从服务器加载
            return null;
        }
    }
}
