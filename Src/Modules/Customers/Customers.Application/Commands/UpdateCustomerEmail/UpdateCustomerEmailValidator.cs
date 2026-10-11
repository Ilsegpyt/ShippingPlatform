using FluentValidation;

namespace Customers.Application.Commands.UpdateCustomerEmail;

public sealed class UpdateCustomerEmailValidator
    : AbstractValidator<UpdateCustomerEmail>
{
    public UpdateCustomerEmailValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(320);
    }
}