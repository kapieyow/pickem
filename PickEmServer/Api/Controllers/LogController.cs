
using Marten;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PickEmServer.Api.Models;
using PickEmServer.Data.Models;
using System.Threading.Tasks;

namespace PickEmServer.Api.Controllers
{
    [Authorize(Policy = "ApiUser")]
    [Produces("application/json")]
    [Route("api/logs")]
    public class LogController : Controller
    {
        private readonly IDocumentStore _documentStore;
        private readonly ILogger<LogController> _logger;

        public LogController(ILogger<LogController> logger, IDocumentStore documentStore)
        {
            _documentStore = documentStore;
            _logger = logger;
        }

        // POST: api/logs
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] LogAdd logAdd)
        {
            // TODO : better mapping e.g. auto mapper or something 
            LogData logData = new LogData
            {
                Component = logAdd.Component,
                LogLevel = logAdd.LogLevel,
                LogMessage = logAdd.LogMessage
            };

            using (var dbSession = _documentStore.LightweightSession())
            {
                dbSession.Store(logData);
                await dbSession.SaveChangesAsync();

                // success return as API "read" object
                // TODO: better mapping e.g.auto mapper or something
                Log newLog = new Log
                {
                    Id = logData.Id,
                    Component = logData.Component,
                    LogLevel = logData.LogLevel,
                    LogMessage = logData.LogMessage
                };

                return new OkObjectResult(newLog);
            }

        }
    }
}