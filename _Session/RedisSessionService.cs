using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System;
using System.Text;
using System.Threading.Tasks;

namespace Api.Config
{
    public class RedisSessionService : ISessionService
    {
        private readonly IDistributedCache _cache;
        private JsonSerializerSettings setting;        

        public RedisSessionService(IDistributedCache cache)
        {
            _cache = cache;
            setting = new JsonSerializerSettings();
            setting.NullValueHandling = NullValueHandling.Ignore;
            setting.DateFormatHandling = DateFormatHandling.MicrosoftDateFormat;
            setting.DateFormatString = "yyyy-MM-dd HH:mm:ss";
        }

        public bool Contain(string token)
        {
            var buffer = _cache.Get(token);
            return buffer != null;
        }

        public async Task<bool> ContainAsync(string token)
        {
            var buffer = await _cache.GetAsync(token);
            return buffer != null;
        }

        public T Get<T>(string token)
        {
            var buffer = _cache.Get(token);
            if (buffer != null)
            {
                var json = Encoding.UTF8.GetString(buffer);
                return JsonConvert.DeserializeObject<T>(json, setting);
            }
            return default(T);
        }

        public async Task<T> GetAsync<T>(string token)
        {
            var buffer = await _cache.GetAsync(token);
            if (buffer != null)
            {
                var json = Encoding.UTF8.GetString(buffer);
                return JsonConvert.DeserializeObject<T>(json, setting);
            }
            return default(T);
        }

        public async Task<bool> RemoveAsync(Session session)
        {
            if (await ContainAsync(session.Token))
            {
                await _cache.RemoveAsync(session.Token);
                return true;
            }
            return false;
        }

        public async Task UpdateAsync(Session session)
        {
            await SetAsync(session);
        }

        public async Task SetAsync(Session session, TimeSpan? timeSpan = null)
        {
            if (timeSpan == null || timeSpan == TimeSpan.MinValue)
            {
                timeSpan = ServerSession.Span;
            }
            var json = JsonConvert.SerializeObject(session, setting);
            await _cache.SetAsync(session.Token
                , Encoding.UTF8.GetBytes(json)
                , new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = timeSpan
                });
        }

        public void Set(Session session, TimeSpan? timeSpan = null)
        {
            if (timeSpan == null || timeSpan == TimeSpan.MinValue)
            {
                timeSpan = ServerSession.Span;
            }
            var json = JsonConvert.SerializeObject(session, setting);
            _cache.Set(session.Token
                , Encoding.UTF8.GetBytes(json)
                , new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = timeSpan
                });
        }

        public void Update(Session session)
        {
            _cache.Refresh(session.Token);
        }

        public bool Remove(Session session)
        {
            _cache.Remove(session.Token);
            return true;
        }
    }
}