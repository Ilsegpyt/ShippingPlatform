using BuildingBlocks.Application;
using Identity.Application.Abstractions;
using Identity.Domain.Repositories;
using MediatR;

namespace Identity.Application.Roles.ActivateRole;

public sealed record ActivateRoleCommand(Guid RoleId)
    : IRequest<Result>;

public sealed class ActivateRoleCommandHandler
    : IRequestHandler<ActivateRoleCommand, Result>
{
    private readonly IRoleRepository _roles;
    private readonly IIdentityUnitOfWork _identityUnitOfWork;

    public ActivateRoleCommandHandler(
        IRoleRepository roles,
        IIdentityUnitOfWork identityUnitOfWork)
    {
        _roles = roles;
        _identityUnitOfWork = identityUnitOfWork;
    }

    public async Task<Result> Handle(
        ActivateRoleCommand request,
        CancellationToken ct)
    {
        var role = await _roles.GetByIdAsync(
            request.RoleId,
            ct);

        if (role is null)
            return Result.Failure("Role not found.");

        role.Activate();

        _roles.Update(role);

        await _identityUnitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }
}