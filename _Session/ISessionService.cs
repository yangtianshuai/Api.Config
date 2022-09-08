using System;
using System.Threading.Tasks;

namespace Api.Config
{
    public interface ISessionService
    {
        /// <summary>
        /// 是否含有Session
        /// </summary>
        /// <param name="token">口令</param>
        /// <returns></returns>
        bool Contain(string token);
        /// <summary>
        /// 是否含有Session
        /// </summary>
        /// <param name="token">口令</param>
        /// <returns></returns>
        Task<bool> ContainAsync(string token);
        /// <summary>
        /// 获取Session
        /// </summary>
        /// <param name="token">口令</param>
        /// <returns></returns>
        T Get<T>(string token);
        /// <summary>
        /// 获取Session
        /// </summary>
        /// <param name="token">口令</param>
        /// <returns></returns>
        Task<T> GetAsync<T>(string token);
        /// <summary>
        /// 设置Session
        /// </summary>
        /// <param name="session">Session对象</param>
        /// <param name="timeSpan">有效时间</param>
        /// <returns></returns>
        Task SetAsync(Session session, TimeSpan? timeSpan = null);
        /// <summary>
        /// 设置Session
        /// </summary>
        /// <param name="session">Session对象</param>
        /// <param name="timeSpan">有效时间</param>
        /// <returns></returns>
        void Set(Session session, TimeSpan? timeSpan = null);
        /// <summary>
        /// 刷新Session
        /// </summary>
        /// <param name="session">Session对象</param>      
        /// <returns></returns>
        Task UpdateAsync(Session session);
        /// <summary>
        /// 刷新Session
        /// </summary>
        /// <param name="session">Session对象</param>      
        /// <returns></returns>
        void Update(Session session);
        /// <summary>
        /// 移除Session
        /// </summary>
        /// <param name="session">Session对象</param>      
        /// <returns></returns>
        Task<bool> RemoveAsync(Session session);
        /// <summary>
        /// 移除Session
        /// </summary>
        /// <param name="session">Session对象</param>      
        /// <returns></returns>
        bool Remove(Session session);
    }
}