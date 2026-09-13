using BuildingBlocks.Application;
using Identity.Domain.Repositories;
using MediatR;

namespace Identity.Application.InternalUsers.GetInternalUsers;

public sealed class GetInternalUsersQueryHandler(
    IInternalUserRepository internalUserRepository,
    IRoleRepository roleRepository)
    : IRequestHandler<
        GetInternalUsersQuery,
        PagedResult<InternalUserResponse>>
{
    public async Task<PagedResult<InternalUserResponse>> Handle(
        GetInternalUsersQuery request,
        CancellationToken ct)
    {
        var page = request.Pagination.PageNumber;
        var pageSize = request.Pagination.PageSize;

        var skip = (page - 1) * pageSize;

        var totalCount =
            await internalUserRepository.CountAsync(ct);

        var users =
            await internalUserRepository.GetAllAsync(
                skip,
                pageSize,
                ct);

        var roleIds = users
            .Select(x => x.RoleId)
            .Distinct()
            .ToList();

        var roles = await roleRepository.GetAllAsync(ct);

        var roleNames = roles
            .Where(x => roleIds.Contains(x.Id))
            .ToDictionary(x => x.Id, x => x.Name);

        var items = users
            .Select(x => new InternalUserResponse(
                x.Id,
                x.UserId,
                x.RoleId,
                roleNames.GetValueOrDefault(x.RoleId, "Unknown"),
                x.Name,
                x.Email,
                x.Phone,
                x.Status))
            .ToList();

        return new PagedResult<InternalUserResponse>(
            items,
            totalCount,
            page,
            pageSize);
    }
}