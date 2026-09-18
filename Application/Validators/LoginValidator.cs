using Application.DTOs.Auth;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Validators
{
    public class LoginValidator : AbstractValidator<LoginDto>
    {
        public LoginValidator()
        {
            RuleFor(x => x.Username)
                .NotEmpty()
                .WithMessage("Username is required.")
                .MaximumLength(100)
                .WithMessage("Username cannot exceed 100 characters.");

            RuleFor(x => x.Password)
                .NotEmpty()
                .WithMessage("Password is required.");
        }
    }
}
