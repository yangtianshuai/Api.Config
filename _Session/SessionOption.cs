using System;
using System.Collections.Generic;

namespace Api.Config
{
    public class SessionOption
    {
        /// <summary>
        /// Token码
        /// </summary>
        public string TokenKey { get; set; }
        /// <summary>
        /// Service类型
        /// </summary>
        public Type Service { get; set; }
        /// <summary>
        /// IP白名单
        /// </summary>
        public IList<string> Origins { get; private set; }
        /// <summary>
        /// 超时时间
        /// </summary>
        public TimeSpan TimeSpan { get; set; } = TimeSpan.MinValue;

        public void WithOrigins(params string[] origins)
        {
            Origins = new List<string>(origins);
        }        
    }
}