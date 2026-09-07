using Identity.Domain.Entities;


namespace Identity.Application.InternalUsers.GetInternalUsers;

public sealed record InternalUserResponse(
    Guid Id,
    Guid UserId,
    Guid RoleId,
    string Name,
    string Email,
    string? Phone,
    InternalUserStatus Status);