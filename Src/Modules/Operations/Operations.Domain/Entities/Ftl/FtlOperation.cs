using BuildingBlocks.Domain;
using Operations.Domain.Common;
using Operations.Domain.Enums;

namespace Operations.Domain.Entities;

public class FtlOperation : Entity<Guid>, IAggregateRoot, ISoftDeletable
{
    private readonly List<IDomainEvent> _domainEvents = new();
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    public string Ref { get; private set; } = null!;
    public int Year { get; private set; }
    public int Sequence { get; private set; }

    public OperationType OperationType { get; private set; }
    public OperationStatus Status { get; private set; } = OperationStatus.Open;

    public Guid? ClientId { get; private set; }
    public string? ClientName { get; private set; }

    public string? BL { get; private set; }
    public string? BookingNumber { get; private set; }
    public ClearanceType? ClearanceType { get; private set; }
    public string? CertificateNumber { get; private set; }

    public int? Packages { get; private set; }
    public decimal? GrossWeight { get; private set; }
    public decimal? Cbm { get; private set; }
    public string? Commodity { get; private set; }

    public string? Terminal { get; private set; }
    public string? Destination { get; private set; }
    public string? AOD { get; private set; }

    public DateOnly? ArrivalDate { get; private set; }
    public TimeOnly? ArrivalTime { get; private set; }

    public DateOnly? RequiredArrivalDate { get; private set; }
    public TimeOnly? RequiredArrivalTime { get; private set; }

    public string? CustomsArea { get; private set; }
    public DateOnly? InvoicesReceivedDate { get; private set; }

    /// <summary>
    /// Serialized JSON for client UI layout state, unmerged fields, etc.
    /// </summary>
    public string? LayoutJson { get; private set; }

    public int Version { get; private set; } = 1;

    public DateOnly Day { get; private set; }
    public TimeOnly Time { get; private set; }
    public string? EnteredByUserId { get; private set; }
    public string? EnteredByName { get; private set; }

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
        string reference,
        int year,
        int sequence,
        OperationType operationType,
        Guid? clientId = null,
        string? clientName = null,
        string? bl = null,
        string? bookingNumber = null,
        ClearanceType? clearanceType = null,
        string? certificateNumber = null,
        int? packages = null,
        decimal? grossWeight = null,
        decimal? cbm = null,
        string? commodity = null,
        string? terminal = null,
        string? destination = null,
        string? aod = null,
        DateOnly? arrivalDate = null,
        TimeOnly? arrivalTime = null,
        DateOnly? requiredArrivalDate = null,
        TimeOnly? requiredArrivalTime = null,
        string? customsArea = null,
        DateOnly? invoicesReceivedDate = null,
        string? layoutJson = null,
        DateOnly? day = null,
        TimeOnly? time = null,
        string? enteredByUserId = null,
        string? enteredByName = null)
        : base(Guid.NewGuid())
    {
        if (string.IsNullOrWhiteSpace(reference))
            throw new ArgumentException("Reference number is required.", nameof(reference));

        Ref = reference.Trim();
        Year = year;
        Sequence = sequence;
        OperationType = operationType;
        Status = OperationStatus.Open;

        ClientId = clientId;
        ClientName = string.IsNullOrWhiteSpace(clientName) ? null : clientName.Trim();
        BL = IdentifierNormalizer.Normalize(bl);
        BookingNumber = IdentifierNormalizer.Normalize(bookingNumber);
        ClearanceType = clearanceType;
        CertificateNumber = IdentifierNormalizer.Normalize(certificateNumber);
        Packages = packages;
        GrossWeight = grossWeight;
        Cbm = cbm;
        Commodity = string.IsNullOrWhiteSpace(commodity) ? null : commodity.Trim();
        Terminal = string.IsNullOrWhiteSpace(terminal) ? null : terminal.Trim();
        Destination = string.IsNullOrWhiteSpace(destination) ? null : destination.Trim();
        AOD = string.IsNullOrWhiteSpace(aod) ? null : aod.Trim();
        ArrivalDate = arrivalDate;
        ArrivalTime = arrivalTime;
        RequiredArrivalDate = requiredArrivalDate;
        RequiredArrivalTime = requiredArrivalTime;
        CustomsArea = string.IsNullOrWhiteSpace(customsArea) ? null : customsArea.Trim();
        InvoicesReceivedDate = invoicesReceivedDate;
        LayoutJson = layoutJson;

        Day = day ?? DateOnly.FromDateTime(DateTime.UtcNow);
        Time = time ?? TimeOnly.FromDateTime(DateTime.UtcNow);
        EnteredByUserId = enteredByUserId;
        EnteredByName = enteredByName;
    }

    public void SetCreatedAudit(string userId, string userName, DateTime utcNow)
    {
        CreatedByUserId = string.IsNullOrWhiteSpace(userId) ? throw new ArgumentException("User ID is required", nameof(userId)) : userId.Trim();
        CreatedByName = string.IsNullOrWhiteSpace(userName) ? "System" : userName.Trim();
        CreatedAtUtc = utcNow.Kind == DateTimeKind.Utc ? utcNow : DateTime.SpecifyKind(utcNow, DateTimeKind.Utc);
    }

    public void UpdateSharedDetails(
        Guid? clientId,
        string? clientName,
        string? bl,
        string? bookingNumber,
        ClearanceType? clearanceType,
        string? certificateNumber,
        int? packages,
        decimal? grossWeight,
        decimal? cbm,
        string? commodity,
        string? terminal,
        string? destination,
        string? aod,
        DateOnly? arrivalDate,
        TimeOnly? arrivalTime,
        DateOnly? requiredArrivalDate,
        TimeOnly? requiredArrivalTime,
        string? customsArea,
        DateOnly? invoicesReceivedDate,
        string? layoutJson)
    {
        ClientId = clientId;
        ClientName = string.IsNullOrWhiteSpace(clientName) ? null : clientName.Trim();
        BL = IdentifierNormalizer.Normalize(bl);
        BookingNumber = IdentifierNormalizer.Normalize(bookingNumber);
        ClearanceType = clearanceType;
        CertificateNumber = IdentifierNormalizer.Normalize(certificateNumber);
        Packages = packages;
        GrossWeight = grossWeight;
        Cbm = cbm;
        Commodity = string.IsNullOrWhiteSpace(commodity) ? null : commodity.Trim();
        Terminal = string.IsNullOrWhiteSpace(terminal) ? null : terminal.Trim();
        Destination = string.IsNullOrWhiteSpace(destination) ? null : destination.Trim();
        AOD = string.IsNullOrWhiteSpace(aod) ? null : aod.Trim();
        ArrivalDate = arrivalDate;
        ArrivalTime = arrivalTime;
        RequiredArrivalDate = requiredArrivalDate;
        RequiredArrivalTime = requiredArrivalTime;
        CustomsArea = string.IsNullOrWhiteSpace(customsArea) ? null : customsArea.Trim();
        InvoicesReceivedDate = invoicesReceivedDate;
        if (layoutJson is not null) LayoutJson = layoutJson;
    }

    public void IncrementVersion()
    {
        Version += 1;
    }

    public void SetLayoutJson(string? layoutJson)
    {
        LayoutJson = layoutJson;
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
        IncrementVersion();

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
