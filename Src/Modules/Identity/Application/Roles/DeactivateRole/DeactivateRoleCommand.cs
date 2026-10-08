using BuildingBlocks.Application;
using Identity.Application.Abstractions;
using Identity.Domain.Repositories;
using MediatR;

namespace Identity.Application.Roles.DeactivateRole;

public sealed record DeactivateRoleCommand(Guid RoleId)
    : IRequest<Result>;

public sealed class DeactivateRoleCommandHandler
    : IRequestHandler<DeactivateRoleCommand, Result>
{
    private readonly IRoleRepository _roles;
    private readonly IIdentityUnitOfWork _identityUnitOfWork;

    public DeactivateRoleCommandHandler(
        IRoleRepository roles,
        IIdentityUnitOfWork identityUnitOfWork)
    {
        _roles = roles;
        _identityUnitOfWork = identityUnitOfWork;
    }

    public async Task<Result> Handle(
        DeactivateRoleCommand request,
        CancellationToken ct)
    {
        var role = await _roles.GetByIdAsync(request.RoleId, ct);

        if (role is null)
            return Result.Failure("Role not found.");

        role.Deactivate();

        _roles.Update(role);

        await _identityUnitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }
}