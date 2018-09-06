using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PickEmServer.Api.Models;
using PickEmServer.Heart;

namespace PickEmServer.Api.Controllers
{
    [Produces("application/json")]
    public class JobController : Controller
    {
        public JobController()
        {
        }

        [Authorize]
        [HttpPost]
        [Route("api/private/jobs")]
        public async Task<IActionResult> RunJob([FromBody] JobRequest jobRequest)
        {
            // TODO: use for jobs or rip out.
            switch (jobRequest.JobTag)
            {
                default:
                    return new BadRequestObjectResult(string.Format("unknown job tag({0})", jobRequest.JobTag));
            }

        }
    }
}