using System;
using System.Collections.Generic;
using System.Text;

namespace Api.Config.Net
{
    public class HttpConfig
    {
        /// <summary>
        /// 超时时间（默认10秒）
        /// </summary>
        public int TimeOut { get; set; } = 10;
        /// <summary>
        /// 默认true只有响应200，可以收到信息
        /// </summary>
        public bool SuccessOnly { get; set; } = true;
        /// <summary>
        /// 正向代理配置
        /// </summary>
        public Dictionary<string, List<string>> Proxys { get; set; } = new Dictionary<string, List<string>>();

    }
}
