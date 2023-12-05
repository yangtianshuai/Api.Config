using System.Collections.Generic;

namespace Api.Config.Open
{
    public class OpenApiJson
    {
        /// <summary>
        /// 自己控制开放的（别人访问我）
        /// </summary>
        public List<OpenApiItem> own_control { get; set; }
        /// <summary>
        /// 他人授权访问的（我访问别人）
        /// </summary>
        public List<OpenApiItem> other_access { get; set; }
    }
}
