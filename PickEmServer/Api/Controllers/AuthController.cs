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
using PickEmServer.Jwt.Models;

namespace PickEmServer.Api.Controllers
{
    [Produces("application/json")]
    [Route("api/auth")]
    public class AuthController : Controller
    {
        private readonly ILogger<AuthController> _logger;
        private readonly JwtIssuerOptions _jwtOptions;
        private readonly UserManager<PickEmUser> _userManager;

        public AuthController(UserManager<PickEmUser> userManager, IOptions<JwtIssuerOptions> jwtOptions, ILogger<AuthController> logger)
        {
            _userManager = userManager;
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

            // get the user to verifty
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
                    new Claim(ClaimTypes.Name, credentials.UserName)
                    // TODO: could add roles here, probably based on user setup in db i.e. kip is god
                };

                var token = new JwtSecurityToken(
                    issuer: _jwtOptions.Issuer,
                    audience: _jwtOptions.Audience,
                    claims: claims,
                    expires: DateTime.Now.AddMinutes(3600), // TODO: Is this a decent timeout?
                    signingCredentials: signinCredentials
                );

                var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
                _logger.LogInformation($"Login success for user ({credentials.UserName})");
                return Ok(new { Token = tokenString });
            }
            else
            {
                _logger.LogWarning("Login attempt failure from user ({0})", credentials.UserName);
                return Unauthorized();
            }
        }
    }
}