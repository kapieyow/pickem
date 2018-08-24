using FluentValidation.Attributes;
using PickEmServer.Api.Models.Validators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PickEmServer.Api.Models
{
    [Validator(typeof(UserRegistrationValidator))]
    public class UserRegistration
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string UserName { get; set; }
        public bool DoNotSetDefaultLeague { get; set; }
    }
}
