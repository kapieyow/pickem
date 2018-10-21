using Microsoft.AspNetCore.Identity;

namespace PickEmServer.App.Models
{
    public class PickEmUser : IdentityUser
    {
        public string DefaultLeagueCode { get; set; }
        public bool IsAGod { get; set; }
    }
}
