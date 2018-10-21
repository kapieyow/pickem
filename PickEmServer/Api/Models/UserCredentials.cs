using FluentValidation.Attributes;
using PickEmServer.Api.Models.Validators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PickEmServer.Api.Models
{
    [Validator(typeof(UserCredentialsValidator))]
    public class UserCredentials
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}
