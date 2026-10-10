
using FluentValidation;

namespace Identity.Application.SubAccounts.SetCustomScope;

public sealed class SetCustomScopeCommandValidator
    : AbstractValidator<SetCustomScopeCommand>
{
    public SetCustomScopeCommandValidator()
    {
        RuleFor(x => x.CustomerId)
            .NotEmpty()
            .WithMessage("Customer ID is required.");

        RuleFor(x => x.SubAccountId)
            .NotEmpty()
            .WithMessage("Sub-account ID is required.");
    }
}
