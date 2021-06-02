using micro_c_web.Shared;
using MicroCLib.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using micro_c_web.Server.Data;
using static MicroCLib.Models.Search;
using micro_c_web.Server.Models;
using Microsoft.AspNetCore.Cors;

namespace micro_c_web.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MicroCenterProxyController : ControllerBase
    {
        private readonly ILogger<MicroCenterProxyController> _logger;
        private readonly ApplicationDbContext _context;

        public MicroCenterProxyController(ILogger<MicroCenterProxyController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        [HttpGet]
        [Route("search")]
        public async Task<SearchResults> Get(string query, string storeId, string categoryFilter, OrderByMode orderBy, int page)
        {
            var results = await Search.LoadQuery(query, storeId, categoryFilter, orderBy, page);
            LoadCachedProperties(ref results);
            return results;
        }

        [HttpGet]
        [Route("searchAll")]
        public async Task<SearchResults> GetAll(string query, string storeId, string categoryFilter, OrderByMode orderBy)
        {
            var results = await Search.LoadAll(query, storeId, categoryFilter, orderBy);
            LoadCachedProperties(ref results);
            return results;
        }

        [HttpGet]
        [Route("get")]
        public async Task<Item> GetOne(string url, string storeId)
        {
            var item = await Item.FromUrl(url, storeId);
            return item;
        }

        private void LoadCachedProperties(ref SearchResults results)
        {
            bool needCache = false;
            foreach(var res in results.Items)
            {
                var cache = _context.ItemCache.FirstOrDefault(i => i.SKU == res.SKU);
                if(cache != null)
                {
                    res.Specs = cache.Specs;
                    res.ComponentType = cache.ProductType;
                    if (cache.PictureUrls != null && cache.PictureUrls.Count > 0)
                    {
                        res.PictureUrls = cache.PictureUrls;
                    }
                    if (res.Price == 0f || res.OriginalPrice == 0f)
                    {
                        res.Price = cache.Price;
                        res.OriginalPrice = cache.Price;
                    }
                }
                else if(!string.IsNullOrWhiteSpace(res.Brand))
                {
                    if(!_context.CacheRequests.Any(r => r.Url == res.URL))
                    {
                        needCache = true;
                        _context.Add(new micro_c_web.Server.Models.ItemCacheRequest()
                        {
                            Url = res.URL
                        });
                    }
                }
            }

            if (needCache)
            {
                _context.SaveChanges();
                Hangfire.BackgroundJob.Enqueue<CacheProcessor>((c) => c.ProcessAllCached());
            }
        }
    }
}
