
using FluentValidation;

namespace Identity.Application.SubAccounts.SetFullScope;

public sealed class SetFullScopeCommandValidator
    : AbstractValidator<SetFullScopeCommand>
{
    public SetFullScopeCommandValidator()
    {
        RuleFor(x => x.CustomerId)
            .NotEmpty()
            .WithMessage("Customer ID is required.");

        RuleFor(x => x.SubAccountId)
            .NotEmpty()
            .WithMessage("Sub-account ID is required.");
    }
}
