using Operations.Application.Abstractions;
using Operations.Domain.Entities;

namespace Operations.Infrastructure.Persistence.Repositories;

public sealed class OperationRepository(
    OperationsDbContext dbContext)
    : IOperationRepository
{
    public async Task AddAsync(
        Operation operation,
        CancellationToken cancellationToken = default)
    {
        await dbContext.Operations.AddAsync(
            operation,
            cancellationToken);
    }
}