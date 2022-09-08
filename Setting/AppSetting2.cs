using System;

namespace Api.Config.Setting
{
    public partial class AppSetting
    {
        internal static SettingOption Option { get; set; }

        /// <summary>
        /// 配置上下文
        /// </summary>
        internal static SettingContext Context { get; set; }


        private static string GetSetting2(string key)
        {            
            return GetSetting2<String>(key);
        }

        private static T GetSetting2<T>(string key)
        {            
            return Context.GetValue<T>(key);
        }

        /// <summary>
        /// 获取运行环境
        /// </summary>
        /// <returns></returns>
        public static AppEnvironment GetEnvironment()
        {
            return Environment;
        }

        internal static void SetEnv(string evn)
        {
            evn = evn.ToLower();
            if (evn == "dev")
            {
                Environment = AppEnvironment.Devolopment;
            }
            else if (evn == "test")
            {
                Environment = AppEnvironment.Test;
            }
            else if (evn == "pro" || evn == "production")
            {
                Environment = AppEnvironment.Production;
            }
        }
    }
}
