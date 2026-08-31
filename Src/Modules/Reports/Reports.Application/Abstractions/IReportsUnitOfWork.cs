
namespace Reports.Application.Abstractions;

public interface IReportsUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken ct = default);
}

