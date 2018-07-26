using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PickEmServer.Api.Models;
using PickEmServer.Heart;

namespace PickEmServer.Api.Controllers
{
    [Produces("application/json")]
    public class JobController : Controller
    {
        private DatabaseInitializer _databaseInitializer;

        public JobController(DatabaseInitializer databaseInitializer)
        {
            _databaseInitializer = databaseInitializer;
        }

        [HttpPost]
        [Route("api/private/jobs")]
        public async Task<IActionResult> RunJob([FromBody] JobRequest jobRequest)
        {
            switch (jobRequest.JobTag)
            {
                case "init-db":
                    return new OkObjectResult(_databaseInitializer.InitDatabase());

                default:
                    return new BadRequestObjectResult(string.Format("unknown job tag({0})", jobRequest.JobTag));
            }

        }
    }
}