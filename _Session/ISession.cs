namespace Api.Config
{
    public interface ISession
    {
        /// <summary>
        /// 是否含有Session
        /// </summary>
        /// <param name="token">口令</param>
        /// <returns></returns>
        bool Contain(string token);
        /// <summary>
        /// 获取Session
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        Session Get(string token);
        /// <summary>
        /// 设置Session
        /// </summary>
        /// <param name="session"></param>
        /// <returns></returns>
        bool Set(Session session);
        /// <summary>
        /// 移除Session
        /// </summary>
        /// <param name="session"></param>
        /// <returns></returns>
        bool Remove(Session session);
    }
}