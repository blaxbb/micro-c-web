using MicroCLib.Models;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static MicroCLib.Models.BuildComponent;

namespace micro_c_web.Client
{
    public class CategoryCache
    {
        public List<Item> Items { get; set; }
        public string Category { get; set; }
        public DateTime Created { get; set; }
        public CategoryCache()
        {
            Created = DateTime.Now;
        }

        public CategoryCache(List<Item> items, ComponentType type)
        {
            Created = DateTime.Now;
            Items = items;
            Category = BuildComponent.CategoryFilterForType(type);
        }

        public async Task Cache(IJSRuntime js)
        {
            Created = DateTime.Now;
            await js.InvokeVoidAsync("AddCacheCategory", this);
        }

        public static async Task<CategoryCache[]> GetCacheCategory(IJSRuntime js, string category)
        {
            return await js.InvokeAsync<CategoryCache[]>("GetCacheCategory", category);
        }
    }
}
