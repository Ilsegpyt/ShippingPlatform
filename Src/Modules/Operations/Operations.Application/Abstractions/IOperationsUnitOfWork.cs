
using BuildingBlocks.Application;

namespace Operations.Application.Abstractions;

public interface IOperationsUnitOfWork : IUnitOfWork
{
    Task BeginTransactionAsync(CancellationToken ct = default);

    Task CommitTransactionAsync(CancellationToken ct = default);

    Task RollbackTransactionAsync(CancellationToken ct = default);
}