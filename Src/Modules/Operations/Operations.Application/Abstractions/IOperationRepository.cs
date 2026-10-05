using Operations.Domain.Entities;

namespace Operations.Application.Abstractions;

public interface IOperationRepository
{
    Task AddAsync(
        Operation operation,
        CancellationToken cancellationToken = default);
}