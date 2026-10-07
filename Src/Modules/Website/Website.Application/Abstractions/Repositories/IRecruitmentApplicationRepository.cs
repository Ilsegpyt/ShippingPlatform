using Website.Domain.Entities;

namespace Website.Application.Abstractions.Repositories;

public interface IRecruitmentApplicationRepository
{
    void Add(RecruitmentApplication application);

    Task<(IReadOnlyList<RecruitmentApplication> Items, int TotalCount)> GetPagedAsync(
     int pageNumber,
     int pageSize,
     CancellationToken ct);

    Task<RecruitmentApplication?> GetByIdAsync(
        int id,
        CancellationToken ct);
}