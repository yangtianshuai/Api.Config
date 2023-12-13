using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;

namespace Api.Config.Setting
{
    public partial class AppSetting
    {
        internal static string ConfigPath { get; set; }
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

        public static bool Set<T>(string key, T value) where T : class
        {
            if (string.IsNullOrEmpty(AppSetting.ConfigPath))
            {
                return false;
            }
            string text = System.IO.File.ReadAllText(AppSetting.ConfigPath);
            var json = JObject.Parse(text);
            var keys = key.Split(':');
            var jtoken = JToken.FromObject(value);
            if (keys.Length == 1)
            {              
                json[keys[0]] = jtoken;
            }
            else if(keys.Length == 2)
            {
                json[keys[0]][keys[1]] = jtoken;
            }
            else if (keys.Length == 3)
            {
                json[keys[0]][keys[1]][keys[2]] = jtoken;
            }
            else if (keys.Length == 4)
            {
                json[keys[0]][keys[1]][keys[2]][keys[3]] = jtoken;
            }
            else
            {
                return false;
            }
            System.IO.File.WriteAllText(AppSetting.ConfigPath, json.ToString());
            return true;
        }
    }
}