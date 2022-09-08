using Microsoft.Extensions.Caching.Memory;
using System;
using System.Threading.Tasks;

namespace Api.Config
{
    /// <summary>
    /// 内存型Session
    /// </summary>
    public class SessionService : ISessionService
    {        
        private readonly IMemoryCache _cache;        

        public SessionService(IMemoryCache cache)
        {         
            _cache = cache;
        }

        public bool Contain(string token)
        {
            if (token == null)
            {
                return false;
            }
            return _cache.Get(token) != null;            
        }

        public async Task<bool> ContainAsync(string token)
        {  
            return await Task.Factory.StartNew(() =>
            {
                return Contain(token);
            }).ConfigureAwait(false);
        }

        public T Get<T>(string token)
        {
            if (token == null)
            {
                return default(T);
            }
            return _cache.Get<T>(token);
        }

        public async Task<T> GetAsync<T>(string token)
        {           
            return await Task.Factory.StartNew(() =>
            {
                return Get<T>(token);
            }).ConfigureAwait(false);
        }

        public bool Remove(Session session)
        {
            try
            {
                _cache.Remove(session.Token);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> RemoveAsync(Session session)
        {
            return await Task.Factory.StartNew(() =>
            {
                return Remove(session);
            }).ConfigureAwait(false);
        }

        public void Set(Session session, TimeSpan? timeSpan = null)
        {
            session.Create_Time = DateTime.Now;
            if (timeSpan == null || timeSpan == TimeSpan.MinValue)
            {
                timeSpan = ServerSession.Span;
            }
            try
            {
                var options = new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = timeSpan
                };
                _cache.Set<Session>(session.Token, session, options);
            }
            catch
            { }
        }       

        public async Task SetAsync(Session session, TimeSpan? timeSpan = null)
        {
            await Task.Factory.StartNew(() =>
            {
                Set(session, timeSpan);
            }).ConfigureAwait(false);
        }

        public void Update(Session session)
        {           
            Remove(session);
            try
            {               
                var options = new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = ServerSession.Span
                };
                _cache.Set(session.Token, session, options);
            }
            catch
            { }
        }

        public async Task UpdateAsync(Session session)
        {
            await Task.Factory.StartNew(() =>
            {
                Update(session);
            }).ConfigureAwait(false);
        }
    }
}