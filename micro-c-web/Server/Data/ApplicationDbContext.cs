using micro_c_web.Server.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace micro_c_web.Server.Data
{
    public class ApplicationDbContext : DbContext
    {
        DbSet<TestItem> TestItems { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            :base(options)
        {
        }
    }
}
