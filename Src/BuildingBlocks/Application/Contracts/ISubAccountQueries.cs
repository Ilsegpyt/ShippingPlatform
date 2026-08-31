public interface ISubAccountQueries
{
    Task<SubAccountAccessInfo?> GetAccessInfoAsync(
        Guid userId,
        CancellationToken ct);
}

public sealed record SubAccountAccessInfo(
    Guid SubAccountId,
    Guid OrganizationId,
    bool IsActive,
    IReadOnlyCollection<string> Permissions,
    bool HasFullScope,
    IReadOnlyCollection<SubAccountScopeInfo> Scopes);

public sealed record SubAccountScopeInfo(
    int Category,
    int Service,
    int ShipmentType);