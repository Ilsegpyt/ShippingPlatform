
using FluentValidation;

namespace Identity.Application.SubAccounts.ResetSubAccountPassword;

public sealed class ResetSubAccountPasswordCommandValidator
    : AbstractValidator<ResetSubAccountPasswordCommand>
{
    public ResetSubAccountPasswordCommandValidator()
    {
        RuleFor(x => x.CustomerId)
            .NotEmpty()
            .WithMessage("Customer ID is required.");

        RuleFor(x => x.SubAccountId)
            .NotEmpty()
            .WithMessage("Sub-account ID is required.");

        RuleFor(x => x.NewPassword)
            .NotEmpty()
            .WithMessage("New password is required.")
            .MinimumLength(8)
            .WithMessage("Password must be at least 8 characters long.");
    }
}
