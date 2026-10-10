using FluentValidation;

namespace Identity.Application.Auth.ActivateAccount;

public sealed class ActivateAccountCommandValidator
    : AbstractValidator<ActivateAccountCommand>
{
    public ActivateAccountCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("User ID is required.");

        RuleFor(x => x.ActivationToken)
            .NotEmpty()
            .WithMessage("Activation token is required.");

        RuleFor(x => x.NewPassword)
            .NotEmpty()
            .WithMessage("New password is required.")
            .MinimumLength(8)
            .WithMessage("Password must be at least 8 characters long.");
    }
}
