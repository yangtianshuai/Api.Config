using System;
using System.Collections.Generic;

namespace Api.Config
{
    /// <summary>
    /// Session
    /// </summary>
    public partial class Session
    {

        public Session()
        {
            Create_Time = DateTime.Now;
        }
        /// <summary>
        /// 角色
        /// </summary>
        public List<string> Roles { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime Create_Time { get; set; }
    }
}