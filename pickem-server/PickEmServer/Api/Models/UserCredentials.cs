using FluentValidation;
using PickEmServer.Api.Models.Validators;

namespace PickEmServer.Api.Models
{
    public class UserCredentials
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}
