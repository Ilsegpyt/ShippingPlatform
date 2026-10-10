
using FluentValidation;

namespace Identity.Application.Roles.GrantPermissionToRole;

public sealed class GrantPermissionToRoleCommandValidator
    : AbstractValidator<GrantPermissionToRoleCommand>
{
    public GrantPermissionToRoleCommandValidator()
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
