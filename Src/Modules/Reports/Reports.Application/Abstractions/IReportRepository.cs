
using Reports.Domain.Report;

namespace Reports.Application.Abstractions;

public interface IReportRepository
{
    Task AddAsync(Report report, CancellationToken ct);

    Task<Report?> GetByIdAsync(
        Guid id,
        CancellationToken ct);

    Task<IReadOnlyList<Report>> GetByCustomerIdAsync(
        Guid customerId,
        CancellationToken ct);
}

