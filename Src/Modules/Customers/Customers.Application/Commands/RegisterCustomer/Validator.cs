using FluentValidation;

namespace Customers.Application.Customers.RegisterCustomer;

public sealed class RegisterCustomerCommandValidator
    : AbstractValidator<RegisterCustomerCommand>
{
    public RegisterCustomerCommandValidator()
    {
        RuleFor(x => x.OwnerName)
            .NotEmpty()
            .WithMessage("Owner name is required.")
            .MaximumLength(200)
            .WithMessage("Owner name must not exceed 200 characters.");

        RuleFor(x => x.CompanyName)
            .NotEmpty()
            .WithMessage("Company name is required.")
            .MaximumLength(200)
            .WithMessage("Company name must not exceed 200 characters.");

        RuleFor(x => x.OwnerPhone)
            .NotEmpty()
            .WithMessage("Owner phone is required.")
            .MaximumLength(50)
            .WithMessage("Owner phone must not exceed 50 characters.");

        RuleFor(x => x.OwnerEmail)
            .NotEmpty()
            .WithMessage("Owner email is required.")
            .EmailAddress()
            .WithMessage("Owner email must be a valid email address.")
            .MaximumLength(320)
            .WithMessage("Owner email must not exceed 320 characters.");

        RuleFor(x => x.Industry)
            .MaximumLength(100)
            .WithMessage("Industry must not exceed 100 characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.Industry));
    }
}