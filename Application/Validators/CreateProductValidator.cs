using Application.DTOs;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Validators
{
    public class CreateProductValidator : AbstractValidator<CreateProductDto>
    {
        public CreateProductValidator()
        {
            RuleFor(x => x.ProductName)
                .NotEmpty()
                .WithMessage("Product name is required.")
                .MaximumLength(255)
                .WithMessage("Product name cannot exceed 255 characters.");
        }
    }
}
