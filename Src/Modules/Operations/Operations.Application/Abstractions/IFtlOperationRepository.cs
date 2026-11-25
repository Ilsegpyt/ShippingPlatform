using Operations.Domain.Entities.Ftl;

namespace Operations.Application.Abstractions;

public interface IFtlOperationRepository
{
    Task<IReadOnlyList<FtlOperation>> GetAllActiveWithDetailsAsync(CancellationToken ct = default);
    Task<IReadOnlyList<FtlOperation>> GetWorkspaceWithDetailsAsync(string? operationType = null, bool onlyActive = false, CancellationToken ct = default);
    Task<FtlOperationsCountsDto> GetCountsAsync(CancellationToken ct = default);
    Task<FtlFilterOptionsDto> GetFilterOptionsAsync(
        string? operationType = null,
        string? status = null,
        bool onlyActive = false,
        bool isDeleted = false,
        CancellationToken ct = default);
    Task<PagedFtlOperationsResult> GetPagedAsync(FtlOperationQueryParams queryParams, CancellationToken ct = default);
    Task<FtlOperation?> GetByIdForUpdateAsync(Guid id, CancellationToken ct = default);
    Task<FtlOperation?> GetByRefAsync(string reference, bool includeDetails = true, CancellationToken ct = default);
    Task<FtlOperation?> GetByIdAsync(Guid id, bool includeDetails = true, CancellationToken ct = default);
    Task AddAsync(FtlOperation operation, CancellationToken ct = default);
    Task AddShipmentAsync(FtlShipment shipment, CancellationToken ct = default);
    Task AddTruckAsync(FtlTruck truck, CancellationToken ct = default);
    Task AddTruckUpdateAsync(FtlTruckUpdate update, CancellationToken ct = default);
    Task AddTripAsync(FtlOperationTrip trip, CancellationToken ct = default);
    Task DeleteShipmentsAsync(IEnumerable<FtlShipment> shipments, CancellationToken ct = default);
    Task DeleteTripsAsync(IEnumerable<FtlOperationTrip> trips, CancellationToken ct = default);
    Task DeleteTrucksAsync(IEnumerable<FtlTruck> trucks, CancellationToken ct = default);
    Task<bool> ExistsByRefAsync(string reference, CancellationToken ct = default);

    Task<bool> ShipmentNumberExistsAsync(string shipNo, Guid? excludeOperationId = null, CancellationToken ct = default);
    Task<bool> HawbExistsAsync(string hawb, Guid? excludeOperationId = null, CancellationToken ct = default);
    Task<bool> MawbHawbPairExistsAsync(string mawb, string hawb, Guid? excludeOperationId = null, CancellationToken ct = default);
    Task<bool> CertShipmentPairExistsAsync(string cert, string shipNo, Guid? excludeOperationId = null, CancellationToken ct = default);
}

