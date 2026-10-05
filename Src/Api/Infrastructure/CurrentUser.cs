using BuildingBlocks.Application.Abstractions;
using Identity.Contracts;
using System.Security.Claims;

namespace Api.Infrastructure;

public sealed class CurrentUser(
    IHttpContextAccessor httpContextAccessor,
    IInternalUserQueries internalUserQueries)
    : ICurrentUser
{
    private string? _userName;

    private ClaimsPrincipal User =>
        httpContextAccessor.HttpContext?.User
        ?? throw new UnauthorizedAccessException("No authenticated user.");

    public Guid UserId
    {
        get
        {
            var value = User.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? User.FindFirstValue("sub");

            return Guid.TryParse(value, out var id)
                ? id
                : throw new UnauthorizedAccessException(
                    "User ID claim is missing or invalid.");
        }
    }

    public async Task<string> GetUserNameAsync(
        CancellationToken ct = default)
    {
        if (_userName is not null)
            return _userName;

        _userName = await internalUserQueries
            .GetNameByUserIdAsync(UserId, ct)
            ?? throw new UnauthorizedAccessException(
                "Internal user name was not found.");

        return _userName;
    }
}