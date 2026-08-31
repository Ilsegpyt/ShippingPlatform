using FluentValidation;

namespace Identity.Application.AccountManagerAssignments.ChangeAccountManager;

public sealed class ChangeAccountManagerCommandValidator
    : AbstractValidator<ChangeAccountManagerCommand>
{
    public ChangeAccountManagerCommandValidator()
    {
        RuleFor(x => x.CustomerId)
            .NotEmpty();

        RuleFor(x => x.NewAccountManagerId)
            .NotEmpty();
    }
}
