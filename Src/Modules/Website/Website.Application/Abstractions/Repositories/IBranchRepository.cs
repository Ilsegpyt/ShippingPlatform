using Website.Domain.Entities;

namespace Website.Application.Abstractions.Repositories;

public interface IBranchRepository
{
    void Add(Branch branch);

    Task<IReadOnlyList<Branch>> GetAllAsync(
        CancellationToken ct);

    Task<Branch?> GetByIdAsync(
        int id,
        CancellationToken ct);

    Task<Branch?> GetForUpdateAsync(
        int id,
        CancellationToken ct);
}