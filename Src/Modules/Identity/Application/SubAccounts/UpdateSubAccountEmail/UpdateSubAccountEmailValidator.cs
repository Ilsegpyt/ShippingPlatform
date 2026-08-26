using FluentValidation;

namespace Identity.Application.SubAccounts.UpdateSubAccountEmail;

public sealed class UpdateSubAccountEmailValidator
    : AbstractValidator<UpdateSubAccountEmailCommand>
{
    public UpdateSubAccountEmailValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(256);
    }
}