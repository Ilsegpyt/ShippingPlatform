using Customers.Application.Commands.UpdateCustomerProfile;
using FluentValidation;

namespace Customers.Application.Customers.UpdateCustomerProfile;

public sealed class UpdateCustomerProfileCommandValidator
    : AbstractValidator<UpdateCustomerProfileCommand>
{
    public UpdateCustomerProfileCommandValidator()
    {
        RuleFor(x => x.CustomerId)
            .NotEmpty()
            .WithMessage("Customer ID is required.");

        RuleFor(x => x.OwnerName)
            .NotEmpty()
            .WithMessage("Owner name is required.");

        RuleFor(x => x.CompanyName)
            .NotEmpty()
            .WithMessage("Company name is required.");

        RuleFor(x => x.OwnerPhone)
            .NotEmpty()
            .WithMessage("Owner phone is required.");

        RuleFor(x => x.Industry)
            .MaximumLength(100)
            .WithMessage("Industry cannot exceed 100 characters.")
            .When(x => x.Industry is not null);
    }
}
