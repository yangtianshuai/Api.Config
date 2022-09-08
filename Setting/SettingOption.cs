using System;

namespace Api.Config.Setting
{
    public class SettingOption
    {        
        /// <summary>
        /// 拉取频次（秒）
        /// </summary>
        public int PullTicks { get; set; } = 10;        
        /// <summary>
        /// 远程下载配置
        /// </summary>
        public Func<SettingContext,string> DowLoad { get; set; }
        
      
        public string Environment
        {
            set
            {
                AppSetting.SetEnv(value);
            }
        }
    }
}
