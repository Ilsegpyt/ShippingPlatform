using BuildingBlocks.Application;
using MediatR;

namespace Identity.Application.InternalUsers.UpdateInternalUserEmail;

public sealed record UpdateInternalUserEmailCommand(
    Guid InternalUserId,
    string Email) : IRequest<Result>;