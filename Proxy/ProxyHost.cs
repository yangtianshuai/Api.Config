using System;
using System.Collections.Generic;
using System.Text;

namespace Api.Config.Proxy
{
    public class ProxyHost
    {
        public string host { get; set; }
        /// <summary>
        /// 速率
        /// </summary>
        public int rate { get; set; }
        /// <summary>
        /// 路由
        /// </summary>
        public List<string> routes { get; set; }
    }
}
