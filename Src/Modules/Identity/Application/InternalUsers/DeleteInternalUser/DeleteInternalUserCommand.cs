using BuildingBlocks.Application;
using MediatR;

namespace Identity.Application.InternalUsers.DeleteInternalUser;

public sealed record DeleteInternalUserCommand(
    Guid InternalUserId) : IRequest<Result>;