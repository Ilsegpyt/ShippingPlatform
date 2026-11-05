using Website.Domain.Entities;

namespace Website.Application.Abstractions.Repositories;

public interface IRecruitmentApplicationRepository
{
    void Add(RecruitmentApplication application);

    Task<(IReadOnlyList<RecruitmentApplication> Items, int TotalCount)> GetPagedAsync(
        int pageNumber,
        int pageSize,
        string? search = null,
        CancellationToken ct = default);

    Task<RecruitmentApplication?> GetByIdAsync(
        int id,
        CancellationToken ct = default);
}
