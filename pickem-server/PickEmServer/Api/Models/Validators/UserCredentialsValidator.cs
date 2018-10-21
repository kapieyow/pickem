using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PickEmServer.Api.Models.Validators
{
    public class UserCredentialsValidator : AbstractValidator<UserCredentials>
    {
        public UserCredentialsValidator()
        {
            RuleFor(creds => creds.UserName).NotEmpty().WithMessage("UserName cannot be empty");
            RuleFor(creds => creds.Password).NotEmpty().WithMessage("Password cannot be empty");
            RuleFor(creds => creds.Password).MinimumLength(6).WithMessage("Password must be at least 6 characters long");
        }
    }
}
