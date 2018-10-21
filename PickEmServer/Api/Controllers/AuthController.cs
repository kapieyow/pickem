using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using PickEmServer.Api.Models;
using PickEmServer.App;
using PickEmServer.App.Models;
using PickEmServer.Heart;
using PickEmServer.Jwt.Models;

namespace PickEmServer.Api.Controllers
{
    [Produces("application/json")]
    [Route("api/auth")]
    public class AuthController : Controller
    {
        private readonly ILogger<AuthController> _logger;
        private readonly JwtIssuerOptions _jwtOptions;
        private readonly LeagueService _leagueService;
        private readonly UserManager<PickEmUser> _userManager;

        public AuthController(UserManager<PickEmUser> userManager, LeagueService leagueService, IOptions<JwtIssuerOptions> jwtOptions, ILogger<AuthController> logger)
        {
            _userManager = userManager;
            _leagueService = leagueService;
            _logger = logger;
            _jwtOptions = jwtOptions.Value;
        }

        // POST api/auth/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserCredentials credentials)
        {
            if (!ModelState.IsValid)
            { 
                return BadRequest(ModelState);
            }

            // get the user to verify
            // NOTE: the find is case INsensitive so "Kip" will find "kip"
            // result is cased correctly for subsequent calls.
            var userToVerify = await _userManager.FindByNameAsync(credentials.UserName);

            if (userToVerify == null)
            {
                _logger.LogWarning("Login attempt from invalid user ({0})", credentials.UserName);
                return Unauthorized();
            }

            // check the credentials
            if (await _userManager.CheckPasswordAsync(userToVerify, credentials.Password))
            {

                // thumbs up, create the Jawt
                var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Consts.SECRET_KEY));
                var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, userToVerify.UserName)
                };
                // TODO probably would be better to use a role manager etc, but none are available for Marten/Postgres yet.
                if ( userToVerify.IsAGod)
                {
                    claims.Add(new Claim(ClaimTypes.Role, Consts.CLAIM_GOD));
                }

                var token = new JwtSecurityToken(
                    issuer: _jwtOptions.Issuer,
                    audience: _jwtOptions.Audience,
                    claims: claims,
                    expires: DateTime.Now.AddMinutes(3600), // TODO: Is this a decent timeout?
                    signingCredentials: signinCredentials
                );

                var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
                _logger.LogInformation($"Login success for unchecked user ({credentials.UserName}) exact user ({userToVerify.UserName})");

                var userLeagues = await _leagueService.ReadUserLeagues(userToVerify.UserName);

                var pickemUser = new UserLoggedIn
                {
                    DefaultLeagueCode = userToVerify.DefaultLeagueCode,
                    Email = userToVerify.Email,
                    UserName = userToVerify.UserName,
                    Token = tokenString,
                    Leagues = userLeagues
                };

                return new OkObjectResult(pickemUser);
            }
            else
            {
                _logger.LogWarning("Login attempt failure from user ({0})", credentials.UserName);
                return Unauthorized();
            }
        }
    }
}