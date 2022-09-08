using Microsoft.AspNetCore.Mvc.Filters;
using System.Collections.Generic;

namespace Api.Config
{
    /// <summary>
    /// 角色过滤
    /// </summary>
    public class RolesAttribute : ActionFilterAttribute
    {
        private List<string> _roles;
        public RolesAttribute(params string[] roles)
        {
            _roles = new List<string>();
            _roles.AddRange(roles);
        }

        public List<string> GetRoles()
        {
            return _roles;
        }
    }
}