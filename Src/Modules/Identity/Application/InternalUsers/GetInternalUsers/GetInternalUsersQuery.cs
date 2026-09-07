using BuildingBlocks.Application;
using MediatR;

namespace Identity.Application.InternalUsers.GetInternalUsers;

public sealed record GetInternalUsersQuery(
    PaginationRequest Pagination)
    : IRequest<PagedResult<InternalUserResponse>>;