namespace BuildingBlocks.Application;

/// <summary>
/// Each module owns exactly one implementation of this (typically its own DbContext),
/// keeping the "each module owns its data" rule enforceable at the code level.
/// </summary>
public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken ct = default);
}

