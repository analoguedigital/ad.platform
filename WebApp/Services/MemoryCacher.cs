using System;
using System.Linq;
using System.Runtime.Caching;

namespace WebApi.Services
{
    public static class MemoryCacher
    {
        /// <summary>
        /// Gets an object from the cache
        /// </summary>
        /// <param name="key">Cache key</param>
        /// <returns></returns>
        public static object GetValue(string key)
        {
            MemoryCache memoryCache = MemoryCache.Default;
            return memoryCache.Get(key);
        }

        /// <summary>
        /// Adds an object to the cache
        /// </summary>
        /// <param name="key">Cache key</param>
        /// <param name="value">Object to be stored</param>
        /// <param name="absExpiration">Cache time</param>
        /// <returns></returns>
        public static bool Add(string key, object value, DateTimeOffset absExpiration)
        {
            MemoryCache memoryCache = MemoryCache.Default;
            return memoryCache.Add(key, value, absExpiration);
        }

        /// <summary>
        /// Removes a cache entry by key
        /// </summary>
        /// <param name="key">Cache key</param>
        public static void Delete(string key)
        {
            MemoryCache memoryCache = MemoryCache.Default;
            if (memoryCache.Contains(key))
                memoryCache.Remove(key);
        }

        /// <summary>
        /// Removes all entries starting with a key
        /// </summary>
        /// <param name="key">Cache key</param>
        public static void DeleteStartingWith(string key)
        {
            MemoryCache memoryCache = MemoryCache.Default;
            var entries = memoryCache.Where(x => x.Key.StartsWith(key)).ToList();
            foreach (var entry in entries)
                memoryCache.Remove(entry.Key);
        }

        /// <summary>
        /// Removes the list entry by key, and the individual entry by id
        /// </summary>
        /// <param name="key">Cache key</param>
        /// <param name="id">Entry ID</param>
        public static void DeleteListAndItem(string key, Guid id)
        {
            MemoryCache memoryCache = MemoryCache.Default;
            if (memoryCache.Contains(key))
                memoryCache.Remove(key);

            var itemKey = $"{key}_{id}";
            if (memoryCache.Contains(itemKey))
                memoryCache.Remove(itemKey);
        }
    }
}