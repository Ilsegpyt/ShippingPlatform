
using FluentValidation;

namespace Identity.Application.SubAccounts.GrantSubAccountPermission;

public sealed class GrantSubAccountPermissionCommandValidator
    : AbstractValidator<GrantSubAccountPermissionCommand>
{
    public GrantSubAccountPermissionCommandValidator()
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
