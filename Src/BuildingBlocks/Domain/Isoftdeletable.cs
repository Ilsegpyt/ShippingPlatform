namespace BuildingBlocks.Domain;

public interface ISoftDeletable
{
    bool IsDeleted { get; }

    DateTime? DeletedAtUtc { get; }

    Guid? DeletedByUserId { get; }

    string? DeletedByName { get; }

    void MarkAsDeleted(
        Guid deletedByUserId,
        string deletedByName,
        DateTime utcNow);
}