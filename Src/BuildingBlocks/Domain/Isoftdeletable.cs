namespace BuildingBlocks.Domain;

/// <summary>
/// Implemented by aggregates that must never be hard-deleted (e.g. Customer, Shipment)
/// because historical/financial/legal traceability must be preserved.
/// EF Core Global Query Filters use IsDeleted to hide soft-deleted rows by default.
/// </summary>
public interface ISoftDeletable
{
    bool IsDeleted { get; }
    DateTime? DeletedAtUtc { get; }
    Guid? DeletedByUserId { get; }

    void MarkAsDeleted(Guid deletedByUserId);
}

