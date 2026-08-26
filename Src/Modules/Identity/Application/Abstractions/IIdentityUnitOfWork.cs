using BuildingBlocks.Application;

namespace Identity.Application.Abstractions;

public interface IIdentityUnitOfWork : IUnitOfWork
{
    Task BeginTransactionAsync(CancellationToken ct = default);

    Task CommitTransactionAsync(CancellationToken ct = default);

    Task RollbackTransactionAsync(CancellationToken ct = default);
}