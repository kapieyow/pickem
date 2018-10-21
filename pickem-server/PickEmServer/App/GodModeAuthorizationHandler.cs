using Microsoft.AspNetCore.Authorization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PickEmServer.App
{
    public class GodModeAuthorizationHandler : AuthorizationHandler<GodModeRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, GodModeRequirement requirement)
        {
            // checking if this user is god
            if ( context.User.Claims.Any(c => c.Type == ClaimTypes.Role && c.Value == Consts.CLAIM_GOD) )
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
