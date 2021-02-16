using micro_c_web.Server.Data;
using micro_c_web.Server.Models;
using MicroCLib.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace micro_c_web.Server.Controllers
{
 
    [ApiController]
    [Route("[controller]")]
    public class ItemCacheController : Controller
    {
        ApplicationDbContext _context;
        public ItemCacheController(ApplicationDbContext context)
        {
            _context = context;
        }

        [Route("requests")]
        [HttpGet]
        public List<ItemCacheRequest> Requests()
        {
            var requests = _context.CacheRequests.ToList();
            return requests;
        }

        [HttpPost]
        [Route("requests")]
        public async Task<ItemCacheRequest> Requests([FromBody] ItemCacheRequest request)
        {
            _context.Add(request);
            await _context.SaveChangesAsync();

            return request;
        }

        [HttpGet]
        public List<ItemCacheEntry> Entries()
        {
            return _context.ItemCache.ToList();
        }

        [HttpGet]
        [Route("process")]
        public async Task<ItemCacheEntry> Process(string sku)
        {
            if (string.IsNullOrWhiteSpace(sku))
            {
                Console.WriteLine("Missing sku");
                return null;
            }

            var results = await Search.LoadAll(sku, "141", null, Search.OrderByMode.match);
            if(results.Items.Count > 0)
            {
                var item = results.Items[0];
                item = await Item.FromUrl(item.URL, "141");
                if(item == null)
                {
                    Console.WriteLine("Item from url failed");
                    return null;
                }

                var entry = new ItemCacheEntry()
                {
                    Created = DateTime.Now,
                    ProductType = item.ComponentType,
                    SKU = item.SKU,
                    Specs = item.Specs
                };
                _context.Add(entry);
                await _context.SaveChangesAsync();
                return entry;
            }

            Console.WriteLine($"Search found no results - {sku}");

            return null;
        }

        [HttpGet]
        [Route("processall")]
        public async Task ProcessAll()
        {
            foreach(var request in _context.CacheRequests.ToList())
            {
                var item = await Item.FromUrl(request.Url, "141");
                if (item == null)
                {
                    Console.WriteLine($"Item from url failed {request.Url}");
                    continue;
                }
                Console.WriteLine($"Caching {item.Name}");

                var duplicates = _context.ItemCache.Where(i => i.SKU == item.SKU);
                foreach(var existing in duplicates)
                {
                    _context.Remove(existing);
                }

                var entry = new ItemCacheEntry()
                {
                    Created = DateTime.Now,
                    ProductType = item.ComponentType,
                    SKU = item.SKU,
                    Specs = item.Specs
                };
                _context.Add(entry);
                _context.Remove(request);
                await Task.Delay(100);
            }
            await _context.SaveChangesAsync();
        }
    }
}
