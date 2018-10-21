
using Marten;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PickEmServer.Api.Models;
using PickEmServer.Data.Models;
using PickEmServer.Heart;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PickEmServer.Api.Controllers
{
    [Produces("application/json")]
    [Route("api/logs")]
    public class LogController : Controller
    {
        private readonly IDocumentStore _documentStore;
        private readonly ILogger<LogController> _logger;
        private readonly LogService _logService;

        public LogController(ILogger<LogController> logger, IDocumentStore documentStore, LogService logService)
        {
            _documentStore = documentStore;
            _logger = logger;
            _logService = logService;
        }

        // POST: api/logs
        [HttpPost]
        public IActionResult Post([FromBody] LogAdd logAdd)
        {
            Log newLog = _logService.AddLog(logAdd);
            return new OkObjectResult(newLog);
        }

        // GET: api/logs
        [Authorize]
        [HttpGet]
        public async Task<List<Log>> Get()
        {
            return await _logService.ReadLogs();
        }
    }
}