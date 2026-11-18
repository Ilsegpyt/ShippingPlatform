using BuildingBlocks.Domain;
using Operations.Domain.Enums;

namespace Operations.Domain.Entities.Ftl;

public class FtlOperation : Entity<Guid>, IAggregateRoot, ISoftDeletable
{
    private readonly List<IDomainEvent> _domainEvents = new();
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    public string Ref => $"OPS-{Id.ToString()[..8].ToUpperInvariant()}";

    public OperationType OperationType { get; private set; }
    public OperationStatus Status { get; private set; } = OperationStatus.Open;

    public int NumberOfShipments { get; private set; } = 1;

    /// <summary>
    /// Serialized JSON for client UI layout state, unmerged fields, etc.
    /// </summary>
    public string? LayoutJson { get; private set; }

    // Concurrency Lock
    public Guid? LockedByUserId { get; private set; }
    public string? LockedByName { get; private set; }
    public DateTime? LockedAtUtc { get; private set; }

    // Soft Delete
    public bool IsDeleted { get; private set; }
    public DateTime? DeletedAtUtc { get; private set; }
    public Guid? DeletedByUserId { get; private set; }
    public string? DeletedByName { get; private set; }

    // Audit
    public DateTime CreatedAtUtc { get; private set; }
    public string CreatedByUserId { get; private set; } = null!;
    public string CreatedByName { get; private set; } = null!;

    // Child Collections
    public ICollection<FtlShipment> Shipments { get; private set; } = new List<FtlShipment>();
    public ICollection<FtlTruck> Trucks { get; private set; } = new List<FtlTruck>();
    public ICollection<FtlOperationTrip> Trips { get; private set; } = new List<FtlOperationTrip>();
    public ICollection<FtlStatusAudit> StatusAudits { get; private set; } = new List<FtlStatusAudit>();

    private FtlOperation() { }

    public FtlOperation(
        Guid id,
        OperationType operationType,
        int numberOfShipments = 1,
        string? layoutJson = null)
        : base(id == Guid.Empty ? Guid.NewGuid() : id)
    {
        OperationType = operationType;
        Status = OperationStatus.Open;
        NumberOfShipments = Math.Max(1, numberOfShipments);
        LayoutJson = layoutJson;
    }

    public void SetCreatedAudit(string userId, string userName, DateTime utcNow)
    {
        CreatedByUserId = string.IsNullOrWhiteSpace(userId) ? throw new ArgumentException("User ID is required", nameof(userId)) : userId.Trim();
        CreatedByName = string.IsNullOrWhiteSpace(userName) ? "System" : userName.Trim();
        CreatedAtUtc = utcNow.Kind == DateTimeKind.Utc ? utcNow : DateTime.SpecifyKind(utcNow, DateTimeKind.Utc);
    }

    public void SetLayoutJson(string? layoutJson)
    {
        LayoutJson = layoutJson;
    }

    public void UpdateNumberOfShipments(int numberOfShipments)
    {
        NumberOfShipments = Math.Max(1, numberOfShipments);
    }

    public void UpdateStatus(
        OperationStatus newStatus,
        string userId,
        string userName,
        DateTime utcNow,
        string? note = null)
    {
        var oldStatus = Status;
        if (oldStatus == newStatus) return;

        Status = newStatus;

        var audit = new FtlStatusAudit(
            Id,
            oldStatus,
            newStatus,
            userId,
            utcNow);

        StatusAudits.Add(audit);
    }

    public bool IsLockActive(DateTime utcNow, int timeoutMinutes = 10)
    {
        if (!LockedByUserId.HasValue || !LockedAtUtc.HasValue)
            return false;

        return LockedAtUtc.Value.AddMinutes(timeoutMinutes) > utcNow;
    }

    public void AcquireLock(Guid userId, string userName, DateTime utcNow)
    {
        LockedByUserId = userId;
        LockedByName = string.IsNullOrWhiteSpace(userName) ? "User" : userName.Trim();
        LockedAtUtc = utcNow.Kind == DateTimeKind.Utc ? utcNow : DateTime.SpecifyKind(utcNow, DateTimeKind.Utc);
    }

    public void ReleaseLock()
    {
        LockedByUserId = null;
        LockedByName = null;
        LockedAtUtc = null;
    }

    public void MarkAsDeleted(Guid deletedByUserId, string deletedByName, DateTime utcNow)
    {
        if (IsDeleted)
            throw new InvalidOperationException("FTL Operation is already deleted.");

        IsDeleted = true;
        DeletedByUserId = deletedByUserId;
        DeletedByName = string.IsNullOrWhiteSpace(deletedByName) ? "System" : deletedByName.Trim();
        DeletedAtUtc = utcNow.Kind == DateTimeKind.Utc ? utcNow : DateTime.SpecifyKind(utcNow, DateTimeKind.Utc);
    }

    public void Restore()
    {
        if (!IsDeleted)
            throw new InvalidOperationException("FTL Operation is not deleted.");

        IsDeleted = false;
        DeletedByUserId = null;
        DeletedByName = null;
        DeletedAtUtc = null;
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
}
