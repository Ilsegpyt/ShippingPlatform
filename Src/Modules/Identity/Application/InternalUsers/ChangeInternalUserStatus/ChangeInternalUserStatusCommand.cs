using BuildingBlocks.Application;
using MediatR;

namespace Identity.Application.InternalUsers.ChangeInternalUserStatus;

public sealed record ChangeInternalUserStatusCommand(
    Guid InternalUserId,
    InternalUserStatusAction Action)
    : IRequest<Result>;