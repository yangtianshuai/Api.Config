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
        private bool access_token_flag = true;
        
        public OpenAttribute(params string[] app_ids)
        {
            _apps = new List<string>();
            _apps.AddRange(app_ids);
        }
        public OpenAttribute(bool access_token_flag, params string[] app_ids) : this(app_ids)
        {
            this.access_token_flag = access_token_flag;
        }

        internal bool AccessToken()
        {
            return this.access_token_flag;
        }

        internal List<string> GetApps()
        {
            var apps = new List<string>();
            if (_apps != null && _apps.Count > 0)
            {
                apps = _apps;
            }                        
            return apps;
        }
    }
}
