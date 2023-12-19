using System.Collections.Generic;

namespace Api.Config.Open
{
    public class OpenApiItem
    {
        public string app_id { get; set; }
        /// <summary>
        /// 服务ID，必须唯一
        /// </summary>
        public string service_id { get; set; }
        /// <summary>
        /// 服务名
        /// </summary>
        public string service_name { get; set; }
        /// <summary>
        /// 访问Token
        /// </summary>
        public string acess_token { get; set; }
        /// <summary>
        /// 路由
        /// </summary>
        public string path { get; set; }        
        /// <summary>
        /// 授权应用
        /// </summary>
        public List<string> apps { get; set; }
    }
}
