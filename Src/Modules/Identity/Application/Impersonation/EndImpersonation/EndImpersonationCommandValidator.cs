using FluentValidation;

namespace Identity.Application.Impersonation.EndImpersonation;

public sealed class EndImpersonationCommandValidator
    : AbstractValidator<EndImpersonationCommand>
{
    public EndImpersonationCommandValidator()
    {
        RuleFor(x => x.AuditLogId)
            .NotEmpty()
            .WithMessage("Audit log ID is required.");

        RuleFor(x => x.ImpersonatorUserId)
            .NotEmpty()
            .WithMessage("Impersonator user ID is required.");
    }
}
