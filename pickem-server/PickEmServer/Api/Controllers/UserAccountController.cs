using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
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
        [Route("api/useraccounts")]
        public async Task<IActionResult> RegisterUser([FromBody] UserRegistration userRegistration)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            string userDefaultLeagueCode = null;
            if (!userRegistration.DoNotSetDefaultLeague)
            {
                if (String.IsNullOrEmpty(userRegistration.DefaultLeagueCode))
                {
                    return new BadRequestObjectResult($"Setting default league, but none passed in");
                }
                else
                {
                    userDefaultLeagueCode = userRegistration.DefaultLeagueCode;
                    var league = await this._leagueService.ReadLeague(userDefaultLeagueCode);
                    // this verifies the exact league casing
                    userDefaultLeagueCode = league.LeagueCode;
                }
            }

            var newPickEmUser = new PickEmUser
            {
                Email = userRegistration.Email,
                UserName = userRegistration.UserName,
                DefaultLeagueCode = userDefaultLeagueCode
            };

            var result = await _userManager.CreateAsync(newPickEmUser, userRegistration.Password);

            if (!result.Succeeded)
                return new BadRequestObjectResult(AuthHelpers.AddErrorsToModelState(result, ModelState));

            if (!userRegistration.DoNotSetDefaultLeague)
            {
                await _leagueService.AddLeaguePlayer(userDefaultLeagueCode, new LeaguePlayerAdd { PlayerTag = userRegistration.UserName, UserName = userRegistration.UserName });
            }

            string resultMessage = string.Format("User ({0}) created", userRegistration.UserName);

            _logger.LogInformation(resultMessage);

            User newUser = new User
            {
                DefaultLeagueCode = userDefaultLeagueCode,
                Email = userRegistration.Email,
                UserName = userRegistration.UserName
            };

            return new OkObjectResult(newUser);
        }

        [Authorize(Policy = "IsAGod")]
        [HttpPut]
        [Route("api/useraccounts/{UserName}")]
        public async Task<User> UpdateUser(string UserName, [FromBody] UserUpdate userUpdates)
        {
            // get the user to verify
            // NOTE: the find is case INsensitive so "Kip" will find "kip"
            // result is cased correctly for subsequent calls.
            var pickEmUser = await _userManager.FindByNameAsync(UserName);

            if (pickEmUser == null)
            {
                throw new ArgumentException($"No matching user to update found for UserName: {UserName}");
            }

            var uncheckedDefaultLeagueCode = userUpdates.DefaultLeagueCode;
            var league = await this._leagueService.ReadLeague(uncheckedDefaultLeagueCode);

            if ( league == null )
            {
                throw new ArgumentException($"No League found for input DefaultLeagueCode: {uncheckedDefaultLeagueCode}");
            }

            // this verifies the exact league casing
            pickEmUser.DefaultLeagueCode = league.LeagueCode;

            await _userManager.UpdateAsync(pickEmUser);

            return new User
            {
                DefaultLeagueCode = pickEmUser.DefaultLeagueCode,
                Email = pickEmUser.Email,
                UserName = pickEmUser.UserName
            };
        }
    }
}
