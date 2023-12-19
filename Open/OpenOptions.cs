using Api.Config.Open;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Api.Config
{
    public class OpenOptions
    {
        ~ OpenOptions()
        {
            OutTime = 0;
        }

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

        public Action<OpenOptions> DownLoad { get; set; }
        internal static ConcurrentDictionary<string, List<string>> OpenApps { get; set; } = new ConcurrentDictionary<string, List<string>>();
        private static ConcurrentDictionary<string, string> OpenTokens { get; set; } = new ConcurrentDictionary<string, string>();
        private static ConcurrentDictionary<string, OpenApiItem> Access { get; set; } = new ConcurrentDictionary<string, OpenApiItem>();
        private static ConcurrentDictionary<string, ServerConfig> AccessServers { get; set; } = new ConcurrentDictionary<string, ServerConfig>();
        
        /// <summary>
        /// 添加服务访问权限
        /// </summary>
        /// <param name="route"></param>
        /// <param name="access_token"></param>
        /// <param name="apps"></param>
        /// <returns></returns>
        public bool Add(string route, string access_token, List<string> apps)
        {
            if (route[0] != '/')
            {
                route = '/' + route;
            }
            if (!string.IsNullOrEmpty(access_token))
            {
                OpenTokens.TryAdd(route, access_token);
            }
            return OpenApps.TryAdd(route, apps);
        }

        /// <summary>
        /// 添加服务访问权限
        /// </summary>
        /// <param name="route"></param>
        /// <param name="apps"></param>
        /// <returns></returns>
        public bool Add(string route, List<string> apps)
        {
            return Add(route, null, apps);
        }

        public void Clear()
        {
            OpenApps.Clear();
            OpenTokens.Clear();
        }

        internal ConcurrentDictionary<string, List<string>> GetApps()
        {
            return OpenApps;
        }

        internal static bool AccessToken(string path,string access_token)
        {
            if (OpenTokens.ContainsKey(path))
            {
                return OpenTokens[path] == access_token;
            }
            return false;
        }

        internal void SetOpen(OpenApiJson json)
        {
            SetOpen(json.own_control);
            SetAccess(json.other_access);
        }
        /// <summary>
        /// 设置开放控制
        /// </summary>
        /// <param name="own_control"></param>
        internal void SetOpen(List<OpenApiItem> own_control)
        {
            Clear();
            own_control.ForEach(item =>
            {
                Add(item.path, item.acess_token, item.apps);
            });
        }
        /// <summary>
        /// 设置访问第三方接口授权
        /// </summary>
        /// <param name="other_access"></param>
        internal void SetAccess(List<OpenApiItem> other_access)
        {
            Access.Clear();
            other_access.ForEach(item =>
            {
                Access.TryAdd(item.service_id, item);
            });
        }

        /// <summary>
        /// 获取第三方接口授权
        /// </summary>
        /// <param name="id"></param>
        internal static OpenApiItem GetAccess(string id)
        {
            if (Access.ContainsKey(id))
            {
                return Access[id];
            }
            return null;
        }
    }


}
