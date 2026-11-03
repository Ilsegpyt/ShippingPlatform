using Operations.Domain.Entities;
using Operations.Domain.Enums;
using Operations.Application.Operations.SearchOperations;

namespace Operations.Application.Abstractions;

public interface IOperationRepository
{
    Task AddAsync(
        Operation operation,
        CancellationToken cancellationToken = default);

    Task<bool> ShipmentNumberExistsAsync(
        string shipmentNumber,
        Guid? excludeOperationId = null,
        CancellationToken cancellationToken = default);

    Task<bool> MblNumberExistsAsync(
        string mblNumber,
        Guid? excludeOperationId = null,
        CancellationToken cancellationToken = default);

    Task<bool> BookingNumberExistsAsync(
        string bookingNumber,
        Guid? excludeOperationId = null,
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

    Task<IReadOnlyList<Operation>> GetDeletedWithDetailsAsync(
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

    // Foreign-Key existence validations for Operation Creation & Updates
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

    Task<OperationContainer?> GetContainerByIdAsync(
        Guid containerId,
        CancellationToken cancellationToken = default);

    Task AddContainerUpdateAsync(
        ContainerUpdate update,
        CancellationToken cancellationToken = default);
}
