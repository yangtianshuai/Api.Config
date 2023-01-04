using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Api.Config.Cache
{
    public abstract class CacheUnit : ICacheUnit
    {
        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <param name="key">键值</param>
        /// <returns></returns>
        public abstract Task<T> GetAsync<T>(string key);
        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <param name="key">键值</param>
        /// <returns></returns>
        public abstract T Get<T>(string key);
        /// <summary>
        /// 设置缓存
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">缓存值</param>
        /// <param name="timeSpan"></param>
        /// <returns></returns>
        public abstract Task<bool> SetAsync(string key, object value, TimeSpan? timeSpan = null);
        /// <summary>
        /// 设置缓存
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">缓存值</param>
        /// <param name="timeSpan"></param>
        /// <returns></returns>
        public abstract bool Set(string key, object value, TimeSpan? timeSpan = null);
        /// <summary>
        /// 清除缓存
        /// </summary>
        /// <param name="key">键</param>  
        /// <returns></returns>
        public abstract Task<bool> ClearAsync(string key);
        /// <summary>
        /// 清除缓存
        /// </summary>
        /// <param name="key">键</param> 
        /// <returns></returns>
        public abstract bool Clear(string key);

        public bool Contain(string key)
        {
            throw new NotImplementedException();
        }
    }
}
