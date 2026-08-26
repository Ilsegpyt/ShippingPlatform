using BuildingBlocks.Application;
using Identity.Application.Abstractions;
using Identity.Domain;
using Identity.Domain.Repositories;
using MediatR;

namespace Identity.Application.Roles.GrantPermissionToRole;

public sealed record GrantPermissionToRoleCommand(Guid RoleId, string PermissionKey) : IRequest<Result>;

public sealed class GrantPermissionToRoleHandler : IRequestHandler<GrantPermissionToRoleCommand, Result>
{
    private readonly IRoleRepository _roles;
    private readonly IIdentityUnitOfWork _identityUnitOfWork;

    public GrantPermissionToRoleHandler(IRoleRepository roles, IIdentityUnitOfWork identityUnitOfWork)
    {
        _roles = roles;
        _identityUnitOfWork = identityUnitOfWork;
    }

    public async Task<Result> Handle(GrantPermissionToRoleCommand request, CancellationToken ct)
    {
        var role = await _roles.GetByIdAsync(request.RoleId, ct);
        if (role is null)
            return Result.Failure("Role not found.");

        var key = PermissionKey.Of(request.PermissionKey);

        if (!PermissionCatalog.All.Contains(key))
            return Result.Failure($"'{request.PermissionKey}' is not a recognized permission key.");

        role.GrantPermission(key);
        _roles.Update(role);
        await _identityUnitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }
}