
using FluentValidation;

namespace Identity.Application.Roles.RevokePermissionFromRole;

public sealed class RevokePermissionFromRoleCommandValidator
    : AbstractValidator<RevokePermissionFromRoleCommand>
{
    public RevokePermissionFromRoleCommandValidator()
    {
        RuleFor(x => x.RoleId)
            .NotEmpty()
            .WithMessage("Role ID is required.");

        RuleFor(x => x.PermissionKey)
            .NotEmpty()
            .WithMessage("Permission key is required.")
            .MaximumLength(200)
            .WithMessage("Permission key cannot exceed 200 characters.");
    }
}
