using System;
using System.Threading.Tasks;

namespace Api.Config.Cache
{
    public interface ICacheUnit
    {
        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <param name="key">键值</param>
        /// <returns></returns>
        Task<T> GetAsync<T>(string key);
        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <param name="key">键值</param>
        /// <returns></returns>
        T Get<T>(string key);
        /// <summary>
        /// 设置缓存
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">缓存值</param>
        /// <param name="timeSpan"></param>
        /// <returns></returns>
        Task<bool> SetAsync(string key, object value, TimeSpan? timeSpan = null);
        /// <summary>
        /// 设置缓存
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">缓存值</param>
        /// <param name="timeSpan"></param>
        /// <returns></returns>
        bool Set(string key, object value, TimeSpan? timeSpan = null);
        /// <summary>
        /// 清除缓存
        /// </summary>
        /// <param name="key">键</param>  
        /// <returns></returns>
        Task<bool> ClearAsync(string key);
        /// <summary>
        /// 清除缓存
        /// </summary>
        /// <param name="key">键</param> 
        /// <returns></returns>
        bool Clear(string key);
    }
}
