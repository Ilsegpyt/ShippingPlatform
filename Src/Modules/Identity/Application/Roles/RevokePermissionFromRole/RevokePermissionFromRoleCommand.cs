using BuildingBlocks.Application;
using Identity.Application.Abstractions;
using Identity.Domain.Repositories;
using Identity.Domain.ValueObjects;
using MediatR;

namespace Identity.Application.Roles.RevokePermissionFromRole;

public sealed record RevokePermissionFromRoleCommand(
    Guid RoleId,
    string PermissionKey)
    : IRequest<Result>;

public sealed class RevokePermissionFromRoleHandler
    : IRequestHandler<RevokePermissionFromRoleCommand, Result>
{
    private readonly IRoleRepository _roles;
    private readonly IIdentityUnitOfWork _identityUnitOfWork;

    public RevokePermissionFromRoleHandler(
        IRoleRepository roles,
        IIdentityUnitOfWork identityUnitOfWork)
    {
        _roles = roles;
        _identityUnitOfWork = identityUnitOfWork;
    }

    public async Task<Result> Handle(
        RevokePermissionFromRoleCommand request,
        CancellationToken ct)
    {
        var role = await _roles.GetByIdAsync(
            request.RoleId,
            ct);

        if (role is null)
            return Result.Failure("Role not found.");

        var key = PermissionKey.Of(request.PermissionKey);

        if (!PermissionCatalog.All.Contains(key))
            return Result.Failure(
                $"'{request.PermissionKey}' is not a recognized permission key.");

        role.RevokePermission(key);

        _roles.Update(role);

        await _identityUnitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }
}