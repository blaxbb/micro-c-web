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

        [HttpGet]
        [Route("getCached/{search}")]
        public Item GetFast(string search)
        {
            var item = new Item();
            var result = new SearchResults() { Items = new List<Item>() { item }, TotalResults = 1 };
            if(search.Length == 6)
            {
                item.SKU = search;
                LoadCachedProperties(ref result);
            }
            else
            {
                item.Specs["UPC"] = search;
                LoadCachedProperties(ref result, CacheMatchMode.UPC);
            }
            return item;
        }

        [HttpGet]
        [Route("getCachedCategory/{category}")]
        public SearchResults GetFastCategory(BuildComponent.ComponentType category)
        {
            var all = _context.ItemCache.Where(i => i.ProductType == category);
            var items = all.Select(i =>
                new Item() {
                    SKU = i.SKU,
                    Name = i.Name,
                    Brand = i.Specs.ContainsKey("Brand") ? i.Specs["Brand"] : "",
                    Specs = i.Specs,
                    ComponentType = i.ProductType,
                    Price = i.Price,
                    OriginalPrice = i.OriginalPrice,
                    PictureUrls = i.PictureUrls,
                    URL = i.Url,
                }
            ).ToList();
            var result = new SearchResults() { Items = items, TotalResults = items.Count };
            return result;
        }

        enum CacheMatchMode
        {
            SKU,
            UPC
        }

        private void LoadCachedProperties(ref SearchResults results, CacheMatchMode mode = CacheMatchMode.SKU)
        {
            bool needCache = false;
            foreach(var res in results.Items)
            {
                ItemCacheEntry cache = null;
                switch (mode)
                {
                    case CacheMatchMode.SKU:
                        cache = _context.ItemCache.FirstOrDefault(i => i.SKU == res.SKU);
                        break;
                    case CacheMatchMode.UPC:
                        cache = _context.ItemCache.ToList().FirstOrDefault(i => i.Specs.ContainsKey("UPC") && res.Specs.ContainsKey("UPC") && i.Specs["UPC"] == res.Specs["UPC"]);
                        break;
                }

                if(cache != null)
                {
                    LoadCachedProperties(res, cache);
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

        private static void LoadCachedProperties(Item res, ItemCacheEntry cache)
        {
            res.Specs = cache.Specs;
            res.ComponentType = cache.ProductType;

            if (string.IsNullOrWhiteSpace(res.SKU))
            {
                res.SKU = cache.SKU;
            }
            if (string.IsNullOrWhiteSpace(res.URL))
            {
                res.URL = cache.Url;
            }
            if (string.IsNullOrWhiteSpace(res.Name))
            {
                res.Name = cache.Name;
            }
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
    }
}
