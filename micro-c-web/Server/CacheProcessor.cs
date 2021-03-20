using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hangfire;
using micro_c_web.Server.Data;
using micro_c_web.Server.Models;

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
            Console.WriteLine("!!!PROCESSING CACHE!!!");
            await PruneCacheRequests();
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

                var entry = new ItemCacheEntry()
                {
                    Created = DateTime.Now,
                    ProductType = item.ComponentType,
                    SKU = item.SKU,
                    Specs = item.Specs,
                    PictureUrls = item.PictureUrls,
                    Url = item.URL
                };
                _context.Add(entry);
                _context.Remove(request);
                await _context.SaveChangesAsync();
                await Task.Delay(100);
            }
        }

        public async Task PrimeStaleItems()
        {
            Console.WriteLine("!!!STALE CHECKING CACHE!!!");
            int staleCount = 0;
            foreach(var cache in _context.ItemCache.ToList().Where(i => DateTime.Now - i.Created > TimeSpan.FromDays(1)))
            {
                if (string.IsNullOrWhiteSpace(cache.Url))
                {
                    _context.Remove(cache);
                }
                else
                {
                    var request = new ItemCacheRequest()
                    {
                        Url = cache.Url
                    };
                    _context.Add(request);
                    staleCount++;
                }
                await _context.SaveChangesAsync();
            }

            if(staleCount > 0)
            {
                Hangfire.BackgroundJob.Enqueue<CacheProcessor>(proc => proc.ProcessAllCached());
            }
        }

        private async Task PruneCacheRequests()
        {
            foreach(var requests in _context.CacheRequests.ToList().GroupBy(r => r.Url))
            {
                var count = requests.Count();
                if(count > 1)
                {
                    for(int i = 0; i < count - 1; i++)
                    {
                        _context.Remove(requests.ElementAt(i));
                    }

                    await _context.SaveChangesAsync();
                }
            }
        }

    }
}
