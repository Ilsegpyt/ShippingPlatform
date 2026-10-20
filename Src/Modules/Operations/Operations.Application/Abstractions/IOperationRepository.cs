using Operations.Domain.Entities;
using Operations.Domain.Enums;
using Operations.Domain.Enums;

namespace Operations.Application.Abstractions;

public interface IOperationRepository
{
    Task AddAsync(
        Operation operation,
        CancellationToken cancellationToken = default);

    Task<bool> DuplicateCheckKeyExistsAsync(
        OperationType operationType,
        string duplicateCheckKey,
        Guid excludeOperationId,
        CancellationToken cancellationToken = default);

    Task<Operation?> GetByIdForUpdateAsync(
        Guid operationId,
        CancellationToken cancellationToken = default);

    Task<Operation?> GetByIdWithDetailsAsync(
        Guid operationId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Operation>> GetAllWithDetailsAsync(
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Operation>> GetDeliveredWithDetailsAsync(
        OperationType? operationType,
        CancellationToken cancellationToken = default);
}
