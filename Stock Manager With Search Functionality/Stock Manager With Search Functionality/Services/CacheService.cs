using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Stock_Manager_With_Search_Functionality.Data;
using Stock_Manager_With_Search_Functionality.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Stock_Manager_With_Search_Functionality.Services
{
    public class CacheService
    {
        private readonly Stock_Manager_With_Search_FunctionalityContext _context;
        private readonly IMemoryCache _cache;

        public static class CacheKeys
        {
            public static string Suppliers { get { return "_Suppliers"; } }
        }
        public CacheService(Stock_Manager_With_Search_FunctionalityContext context, IMemoryCache memoryCache)
        {
            _context = context;
            _cache = memoryCache;
        }

        public class CacheServiceOptions{
            public TimeSpan Expiration { get; set; }
        }

        public static class CacheServiceOptionPreset
        {
            public static CacheServiceOptions Default
            {
                get
                {
                    return new CacheServiceOptions
                    {
                        Expiration = TimeSpan.FromDays(1)
                    };
                }
            }
        }

        public async Task<IEnumerable<Supplier>> SupplierCache(CacheServiceOptions cacheServiceOptions)
        {
            return (IEnumerable<Supplier>) await GetCache(CacheKeys.Suppliers, cacheServiceOptions);
        }

        public async Task<IEnumerable<object>> GetCache(string cacheKey, CacheServiceOptions cacheServiceOptions)
        {
            if (!_cache.TryGetValue(cacheKey, out IEnumerable<object> objects))
            {
                objects = await _context.Supplier.ToListAsync();
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(cacheServiceOptions.Expiration);

                _cache.Set(CacheKeys.Suppliers, objects, cacheEntryOptions);
            }

            return objects;
        }
    }
}
