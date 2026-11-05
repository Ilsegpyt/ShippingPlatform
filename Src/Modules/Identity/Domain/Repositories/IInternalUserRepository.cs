using Identity.Domain.Entities;

namespace Identity.Domain.Repositories;

public interface IInternalUserRepository
{
    Task<InternalUser?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<InternalUser?> GetByUserIdAsync(Guid userId, CancellationToken ct = default);
    void Add(InternalUser internalUser);
    void Update(InternalUser internalUser);
    void Delete(InternalUser internalUser);
    Task<IReadOnlyList<InternalUser>> GetAllAsync(int skip, int take, string? search = null, CancellationToken ct = default);
    Task<IReadOnlyList<InternalUser>> GetAllAsync(int skip, int take, CancellationToken ct) => GetAllAsync(skip, take, null, ct);
    Task<int> CountAsync(string? search = null, CancellationToken ct = default);
    Task<int> CountAsync(CancellationToken ct) => CountAsync(null, ct);

    Task<IReadOnlyList<InternalUser>> GetByIdsAsync(
        IReadOnlyCollection<Guid> ids,
        CancellationToken ct = default);

    Task<IReadOnlyList<InternalUser>> GetByRoleIdAsync(
        Guid roleId,
        CancellationToken ct = default);
}
