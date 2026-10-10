
using FluentValidation;

namespace Identity.Application.InternalUsers.ChangeInternalUserStatus;

public sealed class ChangeInternalUserStatusCommandValidator
    : AbstractValidator<ChangeInternalUserStatusCommand>
{
    public ChangeInternalUserStatusCommandValidator()
    {
        RuleFor(x => x.InternalUserId)
            .NotEmpty()
            .WithMessage("Internal user ID is required.");

        RuleFor(x => x.Action)
            .IsInEnum()
            .WithMessage("Invalid status action.");
    }
}
