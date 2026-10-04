
namespace BuildingBlocks.Domain;

public abstract class SoftDeletableEntity<TId>
    : AuditableEntity<TId>, ISoftDeletable
    where TId : notnull
{
    public bool IsDeleted { get; private set; }

    public DateTime? DeletedAtUtc { get; private set; }

    public Guid? DeletedByUserId { get; private set; }

    protected SoftDeletableEntity()
    {
    }

    protected SoftDeletableEntity(TId id) : base(id)
    {
    }

    public void MarkAsDeleted(Guid deletedByUserId)
    {
        if (IsDeleted)
            return;

        IsDeleted = true;
        DeletedAtUtc = DateTime.UtcNow;
        DeletedByUserId = deletedByUserId;
    }
}
