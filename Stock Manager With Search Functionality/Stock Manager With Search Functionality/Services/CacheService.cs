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
        public async Task<IEnumerable<Supplier>> CacheTryGetValueSet(string cacheKey)
        {

            if (!_cache.TryGetValue(cacheKey, out IEnumerable<Supplier> suppliers))
            {
                Console.WriteLine("Cache not set, fetching -> setting");
                suppliers = await _context.Supplier.ToListAsync();
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromDays(1));

                _cache.Set(CacheKeys.Suppliers, suppliers, cacheEntryOptions);
            }
            else
            {
                Console.WriteLine("Cache was available");
            }

            return suppliers;
        }
    }
}
