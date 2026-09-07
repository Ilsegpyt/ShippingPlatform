using FluentValidation;

namespace Identity.Application.SubAccounts.CreateSubAccount;

public sealed class CreateSubAccountValidator : AbstractValidator<CreateSubAccountCommand>
{
    public CreateSubAccountValidator()
    {
        RuleFor(x => x.OrganizationId).NotEmpty();
        RuleFor(x => x.Email).NotEmpty().EmailAddress();

        RuleFor(x => x.Scopes)
            .Empty()
            .When(x => x.GrantFullScope)
            .WithMessage("Scopes must be empty when GrantFullAccess is true.");

        RuleFor(x => x.Scopes)
            .NotEmpty()
            .When(x => !x.GrantFullScope)
            .WithMessage("At least one scope is required when GrantFullAccess is false.");
    }
}