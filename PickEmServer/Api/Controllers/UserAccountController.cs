using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;
using PickEmServer.Api.Models;
using PickEmServer.App;
using PickEmServer.App.Models;
using PickEmServer.Heart;

namespace PickEmServer.Api.Controllers
{
    [Produces("application/json")]
    [Route("api/useraccounts")]
    public class UserAccountController : Controller
    {
        private readonly ILogger<UserAccountController> _logger;
        private readonly LeagueService _leagueService;
        private readonly UserManager<PickEmUser> _userManager;

        public UserAccountController(ILogger<UserAccountController> logger, UserManager<PickEmUser> userManager, LeagueService leagueService)
        {
            _logger = logger;
            _userManager = userManager;
            _leagueService = leagueService;
        }

        [HttpPost]
        public async Task<IActionResult> RegisterUser([FromBody] UserRegistration userRegistration)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var newPickEmUser = new PickEmUser
            {
                Email = userRegistration.Email,
                UserName = userRegistration.UserName
            };

            var result = await _userManager.CreateAsync(newPickEmUser, userRegistration.Password);

            if (!result.Succeeded)
                return new BadRequestObjectResult(AuthHelpers.AddErrorsToModelState(result, ModelState));

            if (!userRegistration.DoNotSetDefaultLeague)
            {
                // TODO this auto adds use to DEFAULT league. Make this better with registration
                // for specific league etc.
                // ALSO: allow for player tag to not match user name
                await _leagueService.AddLeaguePlayer("Default", new LeaguePlayerAdd { PlayerTag = userRegistration.UserName, UserName = userRegistration.UserName });
            }

            string resultMessage = string.Format("User ({0}) created", userRegistration.UserName);

            _logger.LogInformation(resultMessage);

            User newUser = new User
            {
                Email = userRegistration.Email,
                UserName = userRegistration.UserName
            };

            return new OkObjectResult(newUser);
        }

    }
}