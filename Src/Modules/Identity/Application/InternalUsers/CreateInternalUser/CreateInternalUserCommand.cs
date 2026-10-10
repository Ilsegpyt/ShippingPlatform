

using BuildingBlocks.Application;
using MediatR;

namespace Identity.Application.InternalUsers.CreateInternalUser;

public sealed record CreateInternalUserCommand(
    string Name,
    string Email,
    string Phone,
    Guid RoleId) : IRequest<Result<CreateInternalUserResponse>>;

public sealed record CreateInternalUserResponse(
    Guid InternalUserId);
