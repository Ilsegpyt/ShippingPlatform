namespace BuildingBlocks.Domain;

public abstract class SoftDeletableEntity<TId>
    : AuditableEntity<TId>, ISoftDeletable
    where TId : notnull
{
    public bool IsDeleted { get; private set; }

    public DateTime? DeletedAtUtc { get; private set; }

    public Guid? DeletedByUserId { get; private set; }

    public string? DeletedByName { get; private set; }

    protected SoftDeletableEntity()
    {
    }

    protected SoftDeletableEntity(TId id) : base(id)
    {
    }

    public void MarkAsDeleted(
        Guid deletedByUserId,
        string deletedByName,
        DateTime utcNow)
    {
        if (IsDeleted)
            return;

        IsDeleted = true;
        DeletedAtUtc = utcNow;
        DeletedByUserId = deletedByUserId;
        DeletedByName = deletedByName;
    }
}