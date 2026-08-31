using FluentValidation;

namespace Identity.Application.AccountManagerAssignments.AssignAccountManager;

public sealed class AssignAccountManagerCommandValidator
    : AbstractValidator<AssignAccountManagerCommand>
{
    public AssignAccountManagerCommandValidator()
    {
        RuleFor(x => x.AccountManagerId)
            .NotEmpty();

        RuleFor(x => x.CustomerId)
            .NotEmpty();
    }
}