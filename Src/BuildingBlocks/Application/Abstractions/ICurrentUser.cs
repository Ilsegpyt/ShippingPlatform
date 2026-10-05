namespace BuildingBlocks.Application.Abstractions;

public interface ICurrentUser
{
    Guid UserId { get; }

    Task<string> GetUserNameAsync(
        CancellationToken ct = default);
}