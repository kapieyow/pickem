using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PickEmServer.Api.Models.Validators
{
    public class UserRegistrationValidator : AbstractValidator<UserRegistration>
    {
        public UserRegistrationValidator()
        {
            RuleFor(ur => ur.UserName).NotEmpty().WithMessage("UserName cannot be empty");
            RuleFor(ur => ur.Email).EmailAddress().WithMessage("Email is not a valid email");
            RuleFor(ur => ur.Password).MinimumLength(6).WithMessage("Password must be at least 6 characters long");
        }
    }
}
