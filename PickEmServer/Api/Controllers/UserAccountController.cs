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

namespace PickEmServer.Api.Controllers
{
    [Produces("application/json")]
    [Route("api/useraccounts")]
    public class UserAccountController : Controller
    {
        private readonly ILogger<UserAccountController> _logger;
        private readonly UserManager<PickEmUser> _userManager;

        public UserAccountController(ILogger<UserAccountController> logger, UserManager<PickEmUser> userManager)
        {
            _logger = logger;
            _userManager = userManager;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] UserRegistration userRegistration)
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

            // TODO, should this save a pickem account also (not in ASP identity, in local store?)

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