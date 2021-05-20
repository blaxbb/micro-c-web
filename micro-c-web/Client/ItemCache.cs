using MicroCLib.Models;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace micro_c_web.Client
{
    public class ItemCache
    {
        public Item Item { get; set; }
        public string Category { get; set; }
        public string SKU { get; set; }
        public DateTime Created { get; set; }
        public ItemCache()
        {
            Created = DateTime.Now;
        }

        public ItemCache(Item item)
        {
            Created = DateTime.Now;
            Item = item;
            SKU = item.SKU;
            var cat = item.Categories?.FirstOrDefault();
            Category = cat?.Filter ?? "";
        }

        public async Task Cache(IJSRuntime js)
        {
            Created = DateTime.Now;
            await js.InvokeVoidAsync("AddCacheItem", this);
        }

        public static async Task<ItemCache> GetFromSKU(IJSRuntime js, string sku)
        {
            return await js.InvokeAsync<ItemCache>("GetCacheItem", sku);
        }

        public static async Task<ItemCache[]> GetItemFromCategory(IJSRuntime js, string category)
        {
            return await js.InvokeAsync<ItemCache[]>("GetCacheItemByCategory", category);
        }
    }
}
