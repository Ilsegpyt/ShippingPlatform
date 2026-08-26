
namespace Identity.Domain.Repositories;

public interface IInternalUserRepository
{
    Task<InternalUser?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<InternalUser?> GetByUserIdAsync(Guid userId, CancellationToken ct = default);
    void Add(InternalUser internalUser);
    void Update(InternalUser internalUser);
    void Delete(InternalUser internalUser);
}
