using MicroCLib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static MicroCLib.Models.BuildComponent;

namespace micro_c_web.Server.Models
{
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
    }
}