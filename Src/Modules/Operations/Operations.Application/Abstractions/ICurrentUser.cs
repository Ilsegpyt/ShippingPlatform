namespace Operations.Application.Abstractions;

public interface ICurrentUser
{
    Guid UserId { get; }

    Task<string> GetUserNameAsync(
        CancellationToken ct = default);
}