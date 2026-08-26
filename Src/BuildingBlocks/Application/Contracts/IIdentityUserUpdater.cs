
namespace BuildingBlocks.Application.Contracts;

public interface IIdentityUserUpdater
{
    Task<Result> UpdateEmailAsync(
        Guid userId,
        string email,
        CancellationToken ct = default);
}