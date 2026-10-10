
using FluentValidation;

namespace Identity.Application.SubAccounts.ActivateSubAccount;

public sealed class ActivateSubAccountCommandValidator
    : AbstractValidator<ActivateSubAccountCommand>
{
    public ActivateSubAccountCommandValidator()
    {
        RuleFor(x => x.CustomerId)
            .NotEmpty()
            .WithMessage("Customer ID is required.");

        RuleFor(x => x.SubAccountId)
            .NotEmpty()
            .WithMessage("Sub-account ID is required.");
    }
}
