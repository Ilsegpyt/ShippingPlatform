namespace Identity.Contracts;

public interface IUserAccessQueries
{
    Task<UserAccessInfo?> GetAccessInfoAsync(Guid userId, CancellationToken ct);
    Task<IReadOnlyList<Guid>> GetSuperAdminUserIdsAsync(CancellationToken ct);


}

public sealed record UserAccessInfo(
    bool IsActive,
    string TokenType,
    string? RoleName,
    IReadOnlyCollection<string> Permissions);

