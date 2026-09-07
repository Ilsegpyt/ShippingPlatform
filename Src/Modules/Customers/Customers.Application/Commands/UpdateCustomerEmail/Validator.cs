using FluentValidation;

namespace Customers.Application.Customers.UpdateCustomerEmail;

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