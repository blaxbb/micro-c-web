using MicroCLib.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static MicroCLib.Models.BuildComponent;

namespace micro_c_web.Server.Models
{
    [Index(nameof(SKU))]
    [Index(nameof(UPC))]
    public class ItemCacheEntry
    {
        public long Id { get; set; }
        public string Url { get; set; }
        public DateTime Created { get; set; }
        public string SKU { get; set; }
        public ComponentType ProductType { get; set; }
        public Dictionary<string, string> Specs { get; set; }
        public List<string> PictureUrls { get; set; }
        public float Price { get; set; }
        public float OriginalPrice { get; set; }
        public string Name { get; set; }
        public string Brand { get; set; }
        public string UPC { get; set; }
    }
}