using FluentValidation;

namespace Identity.Application.AccountManagerAssignments.RemoveAccountManager;

public sealed class RemoveAccountManagerCommandValidator
    : AbstractValidator<RemoveAccountManagerCommand>
{
    public RemoveAccountManagerCommandValidator()
    {
        RuleFor(x => x.CustomerId)
            .NotEmpty();
    }
}

