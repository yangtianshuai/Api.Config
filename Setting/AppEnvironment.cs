using System.ComponentModel;

namespace Api.Config.Setting
{
    /// <summary>
    /// 应用环境
    /// </summary>
    public enum AppEnvironment
    {
        /// <summary>
        /// 开发环境
        /// </summary>
        [Description("开发环境")]
        Devolopment,
        /// <summary>
        /// 测试环境
        /// </summary>
        [Description("测试环境")]
        Test,
        /// <summary>
        /// 生产环境
        /// </summary>
        [Description("生产环境")]
        Production
    }
}
