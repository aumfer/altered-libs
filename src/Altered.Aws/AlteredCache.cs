using System;
using System.Collections.Generic;
using System.Runtime.Caching;
using System.Text;
using Altered.Pipeline;
using Altered.Shared;
using Microsoft.Extensions.DependencyInjection;
using Nito.AsyncEx;

namespace Altered.Aws
{
    public sealed class AlteredCache<TRequest, TResponse> : AlteredPipeline<TRequest, TResponse>
        where TResponse: class
    {
        public AlteredCache(IAlteredPipeline<TRequest, TResponse> f, Func<TRequest, string> getKey, CacheItemPolicy cacheItemPolicy, ObjectCache cache, AsyncLock cacheLock) : base(async (request) =>
        {
            var key = $"{f.GetType().GUID}:{getKey(request)}";
            using (await cacheLock.LockAsync())
            {
                var response = cache.Get(key) as TResponse;
                if (response == null)
                {
                    response = await f.Execute(request);
                    cache.AddOrGetExisting(key, response, cacheItemPolicy);
                }
                return response;
            }
        })
        {
        }
    }

    public static class AlteredCacheExtensions
    {
        public static IAlteredPipeline<TRequest, TResponse> WithAlteredCache<TRequest, TResponse>(this IAlteredPipeline<TRequest, TResponse> f, Func<TRequest, string> getKey, CacheItemPolicy cacheItemPolicy = null, ObjectCache cache = null, AsyncLock cacheLock = null)
            where TResponse : class
        {
            cache = cache ?? MemoryCache.Default;
            cacheItemPolicy = cacheItemPolicy ?? DefaultCacheItemPolicy;
            cacheLock = cacheLock ?? new AsyncLock();
            return new AlteredCache<TRequest, TResponse>(f, getKey, cacheItemPolicy, cache, cacheLock);
        }

        static readonly CacheItemPolicy DefaultCacheItemPolicy = new CacheItemPolicy
        {
            SlidingExpiration = TimeSpan.FromMinutes(5.0)
        };
    }
}
