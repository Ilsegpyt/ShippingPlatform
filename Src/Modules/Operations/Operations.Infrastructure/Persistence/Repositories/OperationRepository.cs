using Microsoft.EntityFrameworkCore;
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

    public async Task<Operation?> GetByIdForUpdateAsync(
        Guid operationId,
        CancellationToken cancellationToken = default)
    {
        return await dbContext.Operations
            .Include(x => x.Containers)
                .ThenInclude(x => x.Updates)
            .FirstOrDefaultAsync(
                x => x.Id == operationId,
                cancellationToken);
    }
}