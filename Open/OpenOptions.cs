using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Api.Config
{
    public class OpenOptions
    {
        public static string AppID { get; private set; }
        /// <summary>
        /// 超时时间（秒）
        /// </summary>
        public static int OutTime { get; set; } = 60;

        public void SetAppID(string app_id)
        {
            AppID = app_id;
        }

        public void SetOutTime(int sencond)
        {
            OutTime = sencond;
        }

        internal Action<OpenOptions> Action { get; set; }
        internal static ConcurrentDictionary<string, List<string>> OpenApps { get; set; } = new ConcurrentDictionary<string, List<string>>();

        /// <summary>
        /// 添加引用权限
        /// </summary>
        /// <param name="route">路由</param>
        /// <param name="apps">应用token</param>
        /// <returns></returns>
        public bool Add(string route,List<string> apps)
        {
            if (route[0] != '/')
            {
                route = '/' + route;
            }
            return OpenApps.TryAdd(route, apps);
        }

        public void Clear()
        {
            OpenApps.Clear();
        }

        public ConcurrentDictionary<string, List<string>> GetApps()
        {
            return OpenApps;
        }
    }


}
