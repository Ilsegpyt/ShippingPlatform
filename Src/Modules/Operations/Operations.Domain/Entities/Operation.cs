
using BuildingBlocks.Domain;
using Operations.Domain.Enums;

namespace Operations.Domain.Entities;

public class Operation : SoftDeletableEntity<Guid>, IAggregateRoot
{
    private readonly List<IDomainEvent> _domainEvents = new();

    public IReadOnlyCollection<IDomainEvent> DomainEvents =>
        _domainEvents.AsReadOnly();

    public Guid ClientId { get; private set; }
    public Guid ShippingLineId { get; private set; }
    public ShippingLine ShippingLine { get; private set; } = null!;
    public OperationType OperationType { get; private set; }
    public OperationStatus Status { get; private set; } = OperationStatus.Open;
    public OperationSource Source { get; private set; } = OperationSource.Form;
    public string? ShipmentNumber { get; private set; }
    public string? CertificateNumber { get; private set; }
    public string? DuplicateCheckKey { get; private set; }
    public DateTime? InvoicesReceivedDate { get; private set; }
    public DateTime? DeliveredAtUtc { get; private set; }
    public string? DeliveredByUserId { get; private set; }
    public DateTime? ReopenedAtUtc { get; private set; }
    public string? ReopenedByUserId { get; private set; }
    public string? ReopenReason { get; private set; }

    public int NumberOfContainers { get; private set; }

    public ICollection<OperationContainer> Containers { get; private set; }
        = new List<OperationContainer>();

    public ImportDetails? ImportDetails { get; private set; }
    public ExportDetails? ExportDetails { get; private set; }

    private Operation()
    {
    }

    public Operation(
        Guid clientId,
        Guid shippingLineId,
        OperationType operationType,
        string? shipmentNumber,
        string? certificateNumber,
        DateTime? invoicesReceivedDate)
        : base(Guid.NewGuid())
    {
        ClientId = clientId;
        ShippingLineId = shippingLineId;
        OperationType = operationType;
        ShipmentNumber = shipmentNumber;
        CertificateNumber = certificateNumber;
        InvoicesReceivedDate = invoicesReceivedDate;
        Source = OperationSource.Form;
    }

    public void MarkAsImported()
    {
        Source = OperationSource.Import;
    }

    public void MarkAsFcl()
    {
        if (OperationType != OperationType.Export)
            throw new InvalidOperationException(
                "FCL source is valid only for an Export operation.");

        Source = OperationSource.Fcl;
    }

    public OperationContainer AddFclContainer(
        string containerNumber,
        ContainerType containerType,
        DateOnly requiredLoadingDate,
        TimeOnly requiredLoadingTime)
    {
        if (OperationType != OperationType.Export)
            throw new InvalidOperationException(
                "An FCL container can only be added to an Export operation.");

        var currentCount = Containers.Count;

        if (currentCount >= 200)
            throw new InvalidOperationException(
                "An operation cannot contain more than 200 containers.");

        var nextOrdinal = Containers.Count == 0
            ? (short)1
            : checked((short)(Containers.Max(x => x.Ordinal) + 1));

        if (nextOrdinal > 200)
            throw new InvalidOperationException(
                "No container ordinal is available for this operation.");

        var container = new OperationContainer(
            Id,
            nextOrdinal,
            containerNumber,
            containerType,
            requiredLoadingDate,
            requiredLoadingTime);

        Containers.Add(container);
        NumberOfContainers = Containers.Count;

        return container;
    }

    public void UpdateNumberOfContainers(int count)
    {
        if (count < 1 || count > 200)
            throw new ArgumentOutOfRangeException(
                nameof(count),
                "Number of containers must be between 1 and 200.");

        NumberOfContainers = count;
    }


    public void UpdateDuplicateCheckKey(string? duplicateCheckKey)
    {
        DuplicateCheckKey = string.IsNullOrWhiteSpace(duplicateCheckKey)
            ? null
            : duplicateCheckKey;
    }

    public void UpdateExportShipmentNumber(string? shipmentNumber)
    {
        if (shipmentNumber is null)
            return;

        ShipmentNumber = string.IsNullOrWhiteSpace(shipmentNumber)
            ? null
            : shipmentNumber.Trim();
    }

    public void UpdateExportCertificateNumber(string? certificateNumber)
    {
        if (OperationType != OperationType.Export)
            throw new InvalidOperationException(
                "Certificate details can only be updated for an Export operation.");

        CertificateNumber = string.IsNullOrWhiteSpace(certificateNumber)
            ? null
            : certificateNumber.Trim();
    }

    public void UpdateImportData(
        string? shipmentNumber,
        string? certificateNumber,
        DateTime? invoicesReceivedDate)
    {
        if (shipmentNumber is not null)
            ShipmentNumber = shipmentNumber;

        if (certificateNumber is not null)
            CertificateNumber = certificateNumber;

        if (invoicesReceivedDate.HasValue)
            InvoicesReceivedDate = invoicesReceivedDate.Value;
    }

    public void MarkDelivered(
        string deliveredByUserId,
        DateTime deliveredAtUtc)
    {
        if (Status != OperationStatus.Open)
            throw new InvalidOperationException(
                "Only open operations can be marked as delivered.");

        if (string.IsNullOrWhiteSpace(deliveredByUserId))
            throw new ArgumentException(
                "Delivering user is required.",
                nameof(deliveredByUserId));

        if (deliveredAtUtc.Kind != DateTimeKind.Utc)
            throw new ArgumentException(
                "Delivery timestamp must be UTC.",
                nameof(deliveredAtUtc));

        Status = OperationStatus.Delivered;
        DeliveredByUserId = deliveredByUserId;
        DeliveredAtUtc = deliveredAtUtc;
    }

    public void Reopen(
        string reopenedByUserId,
        string reason,
        DateTime reopenedAtUtc)
    {
        if (Status != OperationStatus.Delivered)
            throw new InvalidOperationException(
                "Only delivered operations can be reopened.");

        if (string.IsNullOrWhiteSpace(reopenedByUserId))
            throw new ArgumentException(
                "Reopening user is required.",
                nameof(reopenedByUserId));

        if (string.IsNullOrWhiteSpace(reason))
            throw new ArgumentException(
                "A reason is required to reopen an operation.",
                nameof(reason));

        if (reason.Trim().Length > 500)
            throw new ArgumentException(
                "Reopen reason cannot exceed 500 characters.",
                nameof(reason));

        if (reopenedAtUtc.Kind != DateTimeKind.Utc)
            throw new ArgumentException(
                "Reopen timestamp must be UTC.",
                nameof(reopenedAtUtc));

        Status = OperationStatus.Open;
        ReopenedByUserId = reopenedByUserId;
        ReopenedAtUtc = reopenedAtUtc;
        ReopenReason = reason.Trim();
    }

    public void SetImportDetails(ImportDetails details)
    {
        ImportDetails = details;
    }

    public void SetExportDetails(ExportDetails details)
    {
        ExportDetails = details;
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
}
