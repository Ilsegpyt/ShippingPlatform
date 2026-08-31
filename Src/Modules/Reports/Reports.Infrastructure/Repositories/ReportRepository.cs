
using Microsoft.EntityFrameworkCore;
using Reports.Application.Abstractions;
using Reports.Domain.Report;
using Reports.Infrastructure.Persistence;

namespace Reports.Infrastructure.Repositories;

public sealed class ReportRepository(
    ReportsDbContext db)
    : IReportRepository
{
    public async Task AddAsync(
        Report report,
        CancellationToken ct)
    {
        await db.Reports.AddAsync(report, ct);
    }

    public async Task<Report?> GetByIdAsync(
        Guid id,
        CancellationToken ct)
    {
        return await db.Reports
            .FirstOrDefaultAsync(x => x.Id == id, ct);
    }

    public async Task<IReadOnlyList<Report>> GetByCustomerIdAsync(
        Guid customerId,
        CancellationToken ct)
    {
        return await db.Reports
            .Where(x => x.CustomerId == customerId)
            .ToListAsync(ct);
    }
}

