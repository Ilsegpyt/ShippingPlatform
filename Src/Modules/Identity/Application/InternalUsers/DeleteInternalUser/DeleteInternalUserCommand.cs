using BuildingBlocks.Application;
using MediatR;

namespace Identity.Application.InternalUsers.DeleteInternalUser;

public sealed record DeleteInternalUsersCommand(
    IReadOnlyCollection<Guid> InternalUserIds)
    : IRequest<Result>;