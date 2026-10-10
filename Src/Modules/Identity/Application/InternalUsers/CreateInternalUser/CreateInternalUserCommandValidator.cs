

using FluentValidation;

namespace Identity.Application.InternalUsers.CreateInternalUser;

public sealed class CreateInternalUserValidator
    : AbstractValidator<CreateInternalUserCommand>
{
    public CreateInternalUserValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x.RoleId)
            .NotEmpty();
    }
}
