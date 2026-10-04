namespace Identity.Contracts;

public interface IInternalUserQueries
{
    Task<string?> GetNameByUserIdAsync(
        Guid userId,
        CancellationToken ct);
}