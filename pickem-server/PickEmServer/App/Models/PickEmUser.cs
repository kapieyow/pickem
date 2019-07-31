using System.Collections.Generic;
using Marten.AspNetIdentity;
using Microsoft.AspNetCore.Identity;

namespace PickEmServer.App.Models
{
    public class PickEmUser : IdentityUser, IClaimsUser
    {
        public string DefaultLeagueCode { get; set; }
        public bool IsAGod { get; set; }
        public IList<string> RoleClaims { get; set; }
    }
}
