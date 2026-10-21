
using Operations.Domain.Entities;
using Operations.Domain.Enums;
using Operations.Application.Operations.SearchOperations;

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

    Task<OperationSearchPage> SearchWithDetailsAsync(
        AdvancedOperationsSearchRequest request,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Operation>> SearchForExportAsync(
        AdvancedOperationsSearchRequest request,
        IReadOnlyCollection<Guid>? operationIds,
        DateTime? createdFromUtc,
        DateTime? createdToUtc,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ShippingLine>> GetActiveShippingLinesAsync(
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Port>> GetActivePortsAsync(
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Transporter>> GetActiveTransportersAsync(
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Driver>> GetActiveDriversAsync(
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Vehicle>> GetActiveVehiclesAsync(
        CancellationToken cancellationToken = default);

    Task<bool> ActiveShippingLineExistsAsync(
        Guid shippingLineId,
        CancellationToken cancellationToken = default);

    Task<bool> ActivePortExistsAsync(
        Guid portId,
        CancellationToken cancellationToken = default);

    Task<bool> ActiveTransporterExistsAsync(
    Guid id,
    CancellationToken cancellationToken = default);

    Task<bool> ActiveDriverExistsAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<bool> ActiveVehicleExistsAsync(
        Guid id,
        CancellationToken cancellationToken = default);

}
