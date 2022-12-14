using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Api.Config.Cache
{
    public class MemoryCacheUnit : CacheUnit
    {
        private readonly IMemoryCache _cache;

        public MemoryCacheUnit(IMemoryCache cache)
        {
            _cache = cache;
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
            return await Task.Run(() =>
            {
                return Clear(key);
            }).ConfigureAwait(false);
        }

        public override T Get<T>(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                return default(T);
            }
            return _cache.Get<T>(key);
        }

        public override async Task<T> GetAsync<T>(string key)
        {
            return await Task.Run(() =>
            {
                return Get<T>(key);
            }).ConfigureAwait(false);
        }

        public override bool Set(string key, object value, TimeSpan? timeSpan = null)
        {            
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
                _cache.Set(key, value, options);
            }
            catch
            {
                return false;
            }
            return true;
        }

        public override async Task<bool> SetAsync(string key, object value, TimeSpan? timeSpan = null)
        {
            return await Task.Run(() =>
            {
                return Set(key, value, timeSpan);
            }).ConfigureAwait(false);
        }
    }
}
