using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace RedisIntro
{
    public static class DistributedCacheExtension
    {
        public static async Task SetRecordAsync<T>(
            this IDistributedCache cache,
            string key,
            T value,
            TimeSpan? absoluteExpiration = null,
            TimeSpan? slidingExpiration = null)
        {
            var opts = new DistributedCacheEntryOptions();
            opts.AbsoluteExpirationRelativeToNow = absoluteExpiration ?? TimeSpan.FromSeconds(60);
            opts.SlidingExpiration = slidingExpiration;

            var jsonData = JsonSerializer.Serialize(value);
            await cache.SetStringAsync(key, jsonData, opts);
        }

        public static async Task<T> GetRecordAsync<T>(
            this IDistributedCache cache,
            string key
            )
        {
            string jsonData = await cache.GetStringAsync(key);

            if (jsonData is null)
            {
                return default(T);
            }

            return JsonSerializer.Deserialize<T>(jsonData);
        }
    }
}
