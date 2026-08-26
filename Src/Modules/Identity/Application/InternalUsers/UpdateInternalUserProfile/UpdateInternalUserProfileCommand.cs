using BuildingBlocks.Application;
using MediatR;

namespace Identity.Application.InternalUsers.UpdateInternalUserProfile;

public sealed record UpdateInternalUserProfileCommand(
    Guid InternalUserId,
    string Name,
    string? Phone) : IRequest<Result>;