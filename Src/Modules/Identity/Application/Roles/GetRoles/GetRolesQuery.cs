using BuildingBlocks.Application;
using Identity.Domain.Entities;
using Identity.Domain.Repositories;
using MediatR;

namespace Identity.Application.Roles.GetRoles;

public sealed record GetRolesQuery
    : IRequest<Result<IReadOnlyList<RoleResponse>>>;

public sealed record RoleResponse(
    Guid Id,
    string Name);

public sealed class GetRolesQueryHandler(
    IRoleRepository roleRepository)
    : IRequestHandler<
        GetRolesQuery,
        Result<IReadOnlyList<RoleResponse>>>
{
    public async Task<Result<IReadOnlyList<RoleResponse>>> Handle(
        GetRolesQuery request,
        CancellationToken ct)
    {
        var roles = await roleRepository.GetAllAsync(ct);

        var result = roles
            .Where(x => x.Status == RoleStatus.Active)
            .OrderBy(x => x.Name)
            .Select(x => new RoleResponse(
                x.Id,
                x.Name))
            .ToList();

        return Result.Success<IReadOnlyList<RoleResponse>>(result);
    }
}