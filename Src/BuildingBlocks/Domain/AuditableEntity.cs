
namespace BuildingBlocks.Domain;

public abstract class AuditableEntity<TId> : Entity<TId>
    where TId : notnull
{
    public DateTime CreatedAtUtc { get; private set; }

    public string CreatedByUserId { get; private set; } = null!;

    public string CreatedByName { get; private set; } = null!;

    public DateTime? UpdatedAtUtc { get; private set; }

    public string? UpdatedByUserId { get; private set; }

    public string? UpdatedByName { get; private set; }

    protected AuditableEntity()
    {
    }

    protected AuditableEntity(TId id) : base(id)
    {
    }

    public void SetCreatedAudit(string userId, string userName, DateTime utcNow)
    {
        CreatedAtUtc = utcNow;
        CreatedByUserId = userId;
        CreatedByName = userName;
    }

    public void SetUpdatedAudit(string userId, string userName, DateTime utcNow)
    {
        UpdatedAtUtc = utcNow;
        UpdatedByUserId = userId;
        UpdatedByName = userName;
    }
}
