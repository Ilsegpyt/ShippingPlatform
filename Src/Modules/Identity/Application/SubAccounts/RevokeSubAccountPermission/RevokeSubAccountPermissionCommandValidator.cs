
using FluentValidation;

namespace Identity.Application.SubAccounts.RevokeSubAccountPermission;

public sealed class RevokeSubAccountPermissionCommandValidator
    : AbstractValidator<RevokeSubAccountPermissionCommand>
{
    public RevokeSubAccountPermissionCommandValidator()
    {
        RuleFor(x => x.CustomerId)
            .NotEmpty()
            .WithMessage("Customer ID is required.");

        RuleFor(x => x.SubAccountId)
            .NotEmpty()
            .WithMessage("Sub-account ID is required.");

        RuleFor(x => x.PermissionKey)
            .NotEmpty()
            .WithMessage("Permission key is required.")
            .MaximumLength(200)
            .WithMessage("Permission key cannot exceed 200 characters.");
    }
}
