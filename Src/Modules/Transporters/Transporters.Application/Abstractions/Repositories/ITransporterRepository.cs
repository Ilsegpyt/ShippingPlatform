using Transporters.Domain.Entities;

namespace Transporters.Application.Abstractions.Repositories;

public interface ITransporterRepository
{
    void Add(Transporter transporter);

    void AddPortCoverage(TransporterPortCoverage portCoverage);

    void AddClientException(TransporterClientException clientException);

    Task<(IReadOnlyList<Transporter> Items, int TotalCount)> GetPagedAsync(
        int pageNumber,
        int pageSize,
        string? search = null,
        CancellationToken ct = default);

    Task<Transporter?> GetByIdAsync(
        Guid id,
        CancellationToken ct = default);

    Task<Transporter?> GetForUpdateAsync(
        Guid id,
        CancellationToken ct = default);

    Task<bool> ExistsAsync(
        Guid id,
        CancellationToken ct = default);

    Task<TransporterPortCoverage?> GetPortCoverageForUpdateAsync(
        Guid transporterId,
        string portName,
        CancellationToken ct = default);

    Task<TransporterPortCoverage?> GetPortCoverageForUpdateAsync(
        Guid portCoverageId,
        CancellationToken ct = default);

    Task<TransporterPortCoverage?> GetPortCoverageByIdAsync(
        Guid transporterId,
        Guid portCoverageId,
        CancellationToken ct = default);
}
