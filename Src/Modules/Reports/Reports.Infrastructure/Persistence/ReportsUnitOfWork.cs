using Reports.Application.Abstractions;

namespace Reports.Infrastructure.Persistence;

public sealed class ReportsUnitOfWork(
    ReportsDbContext db)
    : IReportsUnitOfWork
{
    public Task<int> SaveChangesAsync(
        CancellationToken ct = default)
    {
        return db.SaveChangesAsync(ct);
    }
}