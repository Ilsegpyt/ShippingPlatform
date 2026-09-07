using BuildingBlocks.Application;
using Identity.Application.InternalUsers.GetInternalUsers;
using Identity.Domain.Repositories;
using MediatR;

public sealed class GetInternalUsersQueryHandler(
    IInternalUserRepository internalUserRepository)
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

        var items = users
            .Select(x => new InternalUserResponse(
                x.Id,
                x.UserId,
                x.RoleId,
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