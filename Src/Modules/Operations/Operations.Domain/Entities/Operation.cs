using BuildingBlocks.Domain;
using Operations.Domain.Enums;

namespace Operations.Domain.Entities;

public class Operation : SoftDeletableEntity<Guid>
{
    public Guid ClientId { get; private set; }

    public Guid ShippingLineId { get; private set; }

    public ShippingLine ShippingLine { get; private set; } = null!;

    public OperationType OperationType { get; private set; }

    public OperationStatus Status { get; private set; } = OperationStatus.Open;

    public string? ShipmentNumber { get; private set; }

    public string? CertificateNumber { get; private set; }

    public DateTime? InvoicesReceivedDate { get; private set; }

    public DateTime? DeliveredAtUtc { get; private set; }

    public string? DeliveredByUserId { get; private set; }

    public DateTime? ReopenedAtUtc { get; private set; }

    public string? ReopenedByUserId { get; private set; }

    public string? ReopenReason { get; private set; }

    public ICollection<OperationContainer> Containers { get; private set; }
        = new List<OperationContainer>();

    public ImportDetails? ImportDetails { get; private set; }

    public ExportDetails? ExportDetails { get; private set; }

    private Operation()
    {
    }

    public Operation(OperationType operationType)
    {
        OperationType = operationType;
    }
}