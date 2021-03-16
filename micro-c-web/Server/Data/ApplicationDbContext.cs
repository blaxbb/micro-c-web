using micro_c_web.Server.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace micro_c_web.Server.Data
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<TestItem> TestItems { get; set; }
        public DbSet<ItemCacheRequest> CacheRequests { get; set; }
        public DbSet<ItemCacheEntry> ItemCache { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            :base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<ItemCacheEntry>().Property(i => i.Specs)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, null),
                    v => JsonSerializer.Deserialize<Dictionary<string, string>>(v, null)
                );

            builder.Entity<ItemCacheEntry>().Property(i => i.PictureUrls)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, null),
                    v => JsonSerializer.Deserialize<List<string>>(v, null)
                );

            base.OnModelCreating(builder);
        }
    }
}