using Website.Domain.Entities;

namespace Website.Application.Abstractions.Repositories;

public interface IAgentApplicationRepository
{
    void Add(AgentApplication application);

    Task<(IReadOnlyList<AgentApplication> Items, int TotalCount)> GetPagedAsync(
        int pageNumber,
        int pageSize,
        string? search = null,
        CancellationToken ct = default);

    Task<AgentApplication?> GetByIdAsync(
        int id,
        CancellationToken ct = default);
}
