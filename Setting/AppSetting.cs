using Microsoft.Extensions.Configuration;

namespace Api.Config.Setting
{
    public partial class AppSetting
    {
        internal static IConfiguration Configuration { get; set; }

        /// <summary>
        /// 运行环境(TEST、PRO)
        /// </summary>
        internal static AppEnvironment Environment { get; set; } = AppEnvironment.Devolopment;

        public static string GetSetting(string key)
        {
            string str = "";
            if (Configuration != null)
            {
                str = Configuration[key];
            }
            else if (Option != null)
            {
                ////从平台获取配置
                return GetSetting2(key);
            }
            return str;
        }

        public static T GetSetting<T>(string key)
        {
            if (Configuration != null)
            {
                return Configuration.GetSection(key).Get<T>();
            }
            else if (Option != null)
            {
                //从平台获取配置
                return GetSetting2<T>(key);
            }
            return default(T);
        }
    }
}