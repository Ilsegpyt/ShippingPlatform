using Operations.Domain.Entities;

namespace Operations.Application.Abstractions;

public interface IFtlOperationRepository
{
    Task<IReadOnlyList<FtlOperation>> GetAllActiveWithDetailsAsync(CancellationToken ct = default);
    Task<FtlOperation?> GetByRefAsync(string reference, bool includeDetails = true, CancellationToken ct = default);
    Task<FtlOperation?> GetByIdAsync(Guid id, bool includeDetails = true, CancellationToken ct = default);
    Task<int> GetNextSequenceForYearAsync(int year, CancellationToken ct = default);
    Task<int> GetCurrentWorkspaceVersionAsync(CancellationToken ct = default);
    Task AddAsync(FtlOperation operation, CancellationToken ct = default);
    Task DeleteTripsAsync(IEnumerable<FtlOperationTrip> trips, CancellationToken ct = default);
    Task DeleteTrucksAsync(IEnumerable<FtlTruck> trucks, CancellationToken ct = default);
    Task DeleteShipmentsAsync(IEnumerable<FtlShipment> shipments, CancellationToken ct = default);
    Task<bool> ExistsByRefAsync(string reference, CancellationToken ct = default);
}

