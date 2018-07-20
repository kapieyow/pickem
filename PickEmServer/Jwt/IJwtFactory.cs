using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PickEmServer.Jwt
{
    /// <summary>
    /// TODO : this is ripped off from AngularASPNETCore2WebApiAuth.Helpers
    ///     understand and verify
    /// </summary>
    public interface IJwtFactory
    {
        Task<string> GenerateEncodedToken(string userName, ClaimsIdentity identity);
        ClaimsIdentity GenerateClaimsIdentity(string userName, string id);
    }
}
