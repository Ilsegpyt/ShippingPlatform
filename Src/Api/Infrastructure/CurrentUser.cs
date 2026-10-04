using Identity.Contracts;
using Operations.Application.Abstractions;
using System.Security.Claims;

namespace Api.Infrastructure;

public sealed class CurrentUser(
    IHttpContextAccessor httpContextAccessor,
    IInternalUserQueries internalUserQueries)
    : ICurrentUser
{
    private ClaimsPrincipal User =>
        httpContextAccessor.HttpContext?.User
        ?? throw new UnauthorizedAccessException(
            "No authenticated user.");

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

    public Task<string> GetUserNameAsync(
        CancellationToken ct = default)
    {
        return GetUserNameCoreAsync(ct);
    }

    private async Task<string> GetUserNameCoreAsync(
        CancellationToken ct)
    {
        var name = await internalUserQueries.GetNameByUserIdAsync(
            UserId, ct);

        return name
            ?? throw new UnauthorizedAccessException(
                "Internal user name was not found.");
    }
}