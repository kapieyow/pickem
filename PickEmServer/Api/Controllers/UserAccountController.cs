using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PickEmServer.Api.Models;

namespace PickEmServer.Api.Controllers
{
    [Produces("application/json")]
    [Route("api/useraccounts")]
    public class UserAccountController : Controller
    {
        private ILogger<UserAccountController> _logger;

        public UserAccountController(ILogger<UserAccountController> logger)
        {
            _logger = logger;
        }

        //[HttpPost]
        //public async Task<IActionResult> Post([FromBody] UserRegistration userRegistration)
        //{
        //    if ( !ModelState.IsValid )
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    //var userIdentity = _mapper.Map<AppUser>(model);

        //    //var result = await _userManager.CreateAsync(userIdentity, model.Password);

        //    //if (!result.Succeeded) return new BadRequestObjectResult(Errors.AddErrorsToModelState(result, ModelState));

        //    //await _appDbContext.Customers.AddAsync(new Customer { IdentityId = userIdentity.Id, Location = model.Location });
        //    //await _appDbContext.SaveChangesAsync();

        //    string resultMessage = string.Format("User Account ({0}) created", userRegistration.AccountName);

        //    _logger.LogInformation(resultMessage);

        //    return new OkObjectResult(resultMessage);
        //}
    }
}