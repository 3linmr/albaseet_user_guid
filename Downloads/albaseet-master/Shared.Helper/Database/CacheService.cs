//using Microsoft.Extensions.Caching.Memory;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Shared.Helper.Database
//{
//	public class CacheService
//	{
//		private readonly IMemoryCache _cache;

//		private CacheService(IMemoryCache cache)
//		{
//			_cache = cache;
//		}

//		public static CacheService Instance { get; } = new CacheService(Startup.GetMemoryCache()); // Use a suitable method to access IMemoryCache


//		public static T? GetOrSet<T>(this IMemoryCache cache, string key, Func<T> valueFactory, TimeSpan? expiration = null)
//		{
//			// Use a lock to ensure thread-safe access
//			var lockObject = new object();
//			lock (lockObject)
//			{
//				// Check if the value is already cached
//				T? value = cache.Get<T>(key);
//				if (value != null)
//				{
//					return value;
//				}

//				// Value not cached, create it using the valueFactory
//				value = valueFactory();

//				// Store the value in the cache with optional expiration
//				if (expiration.HasValue)
//				{
//					cache.Set(key, value, expiration.Value);
//				}
//				else
//				{
//					cache.Set(key, value);
//				}

//				return value;
//			}
//		}
//	}
//}
