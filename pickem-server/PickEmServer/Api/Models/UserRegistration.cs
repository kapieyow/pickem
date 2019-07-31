using FluentValidation;
using PickEmServer.Api.Models.Validators;

namespace PickEmServer.Api.Models
{
    public class UserRegistration
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string UserName { get; set; }
        public bool DoNotSetDefaultLeague { get; set; }
        public string DefaultLeagueCode { get; set; }
    }
}
