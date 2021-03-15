using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hangfire;
using micro_c_web.Server.Data;

namespace micro_c_web.Server
{
    public class CacheProcessor
    {
        private ApplicationDbContext _context;
        public CacheProcessor(ApplicationDbContext dbcontext)
        {
            _context = dbcontext;
        }

        [Queue("cache")]
        public async Task ProcessAllCached()
        {
            foreach (var request in _context.CacheRequests.ToList())
            {
                var item = await MicroCLib.Models.Item.FromUrl(request.Url, "141");
                if (item == null)
                {
                    Console.WriteLine($"Item from url failed {request.Url}");
                    continue;
                }
                Console.WriteLine($"Caching {item.Name}");

                var duplicates = _context.ItemCache.Where(i => i.SKU == item.SKU);
                foreach (var existing in duplicates)
                {
                    _context.Remove(existing);
                }

                var entry = new micro_c_web.Server.Models.ItemCacheEntry()
                {
                    Created = DateTime.Now,
                    ProductType = item.ComponentType,
                    SKU = item.SKU,
                    Specs = item.Specs
                };
                _context.Add(entry);
                _context.Remove(request);
                await _context.SaveChangesAsync();
                await Task.Delay(100);
            }
        }

    }
}
