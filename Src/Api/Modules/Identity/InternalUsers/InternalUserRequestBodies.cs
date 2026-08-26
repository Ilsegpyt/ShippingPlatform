namespace Identity.Api.InternalUsers;

public sealed record UpdateInternalUserEmailRequestBody(
    string Email);

public sealed record UpdateInternalUserProfileRequestBody(
    string Name,
    string? Phone);