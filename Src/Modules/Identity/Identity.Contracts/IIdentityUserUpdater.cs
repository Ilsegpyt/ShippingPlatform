
using BuildingBlocks.Application;

namespace Identity.Contracts;

public interface IIdentityUserUpdater
{
    Task<Result> UpdateEmailAsync(
        Guid userId,
        string email,
        CancellationToken ct = default);
}