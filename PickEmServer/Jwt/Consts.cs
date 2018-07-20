using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

/// <summary>
/// TODO : this is ripped off from AngularASPNETCore2WebApiAuth.Helpers
///     understand and verify
/// </summary>
namespace PickEmServer.Jwt
{
    public static class JwtClaimIdentifiers
    {
        public const string Rol = "rol", Id = "id";
    }

    public static class JwtClaims
    {
        public const string ApiAccess = "api_access";
    }
}
