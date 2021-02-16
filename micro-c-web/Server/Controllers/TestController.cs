using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using micro_c_web.Server.Models;
using micro_c_web.Server.Data;
using System.Text.Json;

namespace micro_c_web.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TestController : Controller
    {
        private readonly ApplicationDbContext _context;
        public TestController(ApplicationDbContext context)
        {
            _context = context;
        }


        [HttpGet]
        public async Task<string> Test()
        {
            var json = JsonSerializer.Serialize(_context.TestItems.ToList());
            return $"AFSFSAF - {json}";
        }

        [HttpPost]
        public async Task<string> Create(string name)
        {
            var item = new TestItem()
            {
                Name = name
            };

            _context.Add(item);
            await _context.SaveChangesAsync();

            var json = JsonSerializer.Serialize(_context.TestItems.ToList());
            return $"AFSFSAF - {json}";
        }
    }
}
