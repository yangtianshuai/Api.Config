using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Api.Config.Cache
{
    public class RedisCacheUnit : CacheUnit
    {
        private readonly IDistributedCache _cache;
        private JsonSerializerSettings setting;

        public RedisCacheUnit(IDistributedCache cache)
        {
            _cache = cache;
            setting = new JsonSerializerSettings();
            setting.NullValueHandling = NullValueHandling.Ignore;
            setting.DateFormatHandling = DateFormatHandling.MicrosoftDateFormat;
            setting.DateFormatString = "yyyy-MM-dd HH:mm:ss";
        }

        public override bool Clear(string key)
        {
            try
            {
                _cache.Remove(key);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public override async Task<bool> ClearAsync(string key)
        {
            if (Contain(key))
            {
                await _cache.RemoveAsync(key);
                return true;
            }
            return false;
        }

        public override T Get<T>(string key)
        {
            var buffer = _cache.Get(key);
            if (buffer != null)
            {
                var json = Encoding.UTF8.GetString(buffer);
                return JsonConvert.DeserializeObject<T>(json, setting);
            }
            return default(T);
        }

        public override async Task<T> GetAsync<T>(string key)
        {
            var buffer = await _cache.GetAsync(key);
            if (buffer != null)
            {
                var json = Encoding.UTF8.GetString(buffer);
                return JsonConvert.DeserializeObject<T>(json, setting);
            }
            return default(T);
        }

        public override bool Set(string key, object value, TimeSpan? timeSpan = null)
        {
            if (timeSpan == null || timeSpan == TimeSpan.MinValue)
            {
                timeSpan = ServerSession.Span;
            }
            var json = JsonConvert.SerializeObject(value, setting);
            _cache.Set(key
                , Encoding.UTF8.GetBytes(json)
                , new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = timeSpan
                });
            return true;
        }

        public override async Task<bool> SetAsync(string key, object value, TimeSpan? timeSpan = null)
        {
            if (timeSpan == null || timeSpan == TimeSpan.MinValue)
            {
                timeSpan = ServerSession.Span;
            }
            var json = JsonConvert.SerializeObject(value, setting);
            await _cache.SetAsync(key
                , Encoding.UTF8.GetBytes(json)
                , new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = timeSpan
                });
            return true;
        }
    }
}
