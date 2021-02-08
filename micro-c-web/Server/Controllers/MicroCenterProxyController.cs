using micro_c_web.Shared;
using MicroCLib.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static MicroCLib.Models.Search;

namespace micro_c_web.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MicroCenterProxyController : ControllerBase
    {
        private readonly ILogger<MicroCenterProxyController> _logger;

        public MicroCenterProxyController(ILogger<MicroCenterProxyController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        [Route("search")]
        public async Task<SearchResults> Get(string query, string storeId, string categoryFilter, OrderByMode orderBy, int page)
        {
            return await Search.LoadQuery(query, storeId, categoryFilter, orderBy, page);
        }

        [HttpGet]
        [Route("searchAll")]
        public async Task<SearchResults> GetAll(string query, string storeId, string categoryFilter, OrderByMode orderBy)
        {
            return await Search.LoadAll(query, storeId, categoryFilter, orderBy);
        }
    }
}
