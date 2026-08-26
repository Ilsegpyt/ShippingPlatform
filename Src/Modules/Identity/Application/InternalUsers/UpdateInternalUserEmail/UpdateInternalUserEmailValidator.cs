using FluentValidation;

namespace Identity.Application.InternalUsers.UpdateInternalUserEmail;

public sealed class UpdateInternalUserEmailValidator
    : AbstractValidator<UpdateInternalUserEmailCommand>
{
    public UpdateInternalUserEmailValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(256);
    }
}