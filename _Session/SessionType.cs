using System.ComponentModel;

namespace Api.Config
{
    public enum SessionType
    {
        /// <summary>
        /// 内存
        /// </summary>
        [Description("内存")]
        Memory = 1,
        /// <summary>
        /// Redis
        /// </summary>
        [Description("Redis")]
        Redis = 2
    }
}