using BuildingBlocks.Application;
using Identity.Domain.Repositories;
using MediatR;

namespace Identity.Application.Roles.GetRolePermissions;

public sealed record GetRolePermissionsQuery(
    Guid RoleId)
    : IRequest<Result<IReadOnlyList<string>>>;

public sealed class GetRolePermissionsQueryHandler(
    IRoleRepository roleRepository)
    : IRequestHandler<
        GetRolePermissionsQuery,
        Result<IReadOnlyList<string>>>
{
    public async Task<Result<IReadOnlyList<string>>> Handle(
        GetRolePermissionsQuery request,
        CancellationToken ct)
    {
        var role = await roleRepository.GetByIdAsync(
            request.RoleId,
            ct);

        if (role is null)
            return Result.Failure<IReadOnlyList<string>>(
                "Role not found.");

        var permissions = role.Permissions
            .Select(x => x.Value)
            .OrderBy(x => x)
            .ToList();

        return Result.Success<IReadOnlyList<string>>(
            permissions);
    }
}