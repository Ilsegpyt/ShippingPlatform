
using BuildingBlocks.Domain;
using Operations.Domain.Enums;

namespace Operations.Domain.Entities;

public class OperationContainer : SoftDeletableEntity<Guid>
{
    public Guid OperationId { get; private set; }

    public short Ordinal { get; private set; }

    public string? ContainerNumber { get; private set; }

    public ContainerType? ContainerType { get; private set; }

    // Import and Export: each container has its own destination.
    public string? Destination { get; private set; }

    // Import only: each container has its own free time.
    public DateOnly? FreeTimeTill { get; private set; }

    public DateOnly? RequiredLoadingDate { get; private set; }

    public TimeOnly? RequiredLoadingTime { get; private set; }

    public DateOnly? ActualArrivalDate { get; private set; }

    public TimeOnly? ActualArrivalTime { get; private set; }

    public DateOnly? GateOutDate { get; private set; }

    public TimeOnly? GateOutTime { get; private set; }

    public string? MissingDocs { get; private set; }

    public DateOnly? MissingDocsReceivedDate { get; private set; }

    public DateTime? CheckOutFromThePort { get; private set; }

    public Guid? TransporterId { get; private set; }

    public Transporter? Transporter { get; private set; }

    public Guid? DriverId { get; private set; }

    public Driver? Driver { get; private set; }

    public Guid? VehicleId { get; private set; }

    public Vehicle? Vehicle { get; private set; }

    public Operation Operation { get; private set; } = null!;

    public ICollection<ContainerUpdate> Updates { get; private set; }
        = new List<ContainerUpdate>();

    private OperationContainer()
    {
    }

    public OperationContainer(Guid operationId, short ordinal)
    {
        if (operationId == Guid.Empty)
            throw new ArgumentException("Operation is required.", nameof(operationId));

        if (ordinal < 1 || ordinal > 200)
            throw new ArgumentOutOfRangeException(
                nameof(ordinal),
                "Container ordinal must be between 1 and 200.");

        OperationId = operationId;
        Ordinal = ordinal;
    }

    public OperationContainer(
        Guid operationId,
        short ordinal,
        string containerNumber,
        ContainerType containerType,
        DateOnly requiredLoadingDate,
        TimeOnly requiredLoadingTime)
        : this(operationId, ordinal)
    {
        if (string.IsNullOrWhiteSpace(containerNumber))
            throw new ArgumentException(
                "Container number is required.",
                nameof(containerNumber));

        if (containerNumber.Trim().Length > 11)
            throw new ArgumentException(
                "Container number cannot exceed 11 characters.",
                nameof(containerNumber));

        ContainerNumber = containerNumber.Trim();
        ContainerType = containerType;
        RequiredLoadingDate = requiredLoadingDate;
        RequiredLoadingTime = requiredLoadingTime;
    }

    public void UpdateExportData(
        ContainerType? containerType,
        string? containerNumber,
        DateOnly? requiredLoadingDate,
        TimeOnly? requiredLoadingTime,
        string? destination)
    {
        if (containerType.HasValue)
            ContainerType = containerType.Value;

        if (containerNumber is not null)
        {
            var normalized = containerNumber.Trim();

            if (normalized.Length == 0)
                throw new ArgumentException(
                    "Container number cannot be empty.",
                    nameof(containerNumber));

            if (normalized.Length > 11)
                throw new ArgumentException(
                    "Container number cannot exceed 11 characters.",
                    nameof(containerNumber));

            ContainerNumber = normalized;
        }

        if (requiredLoadingDate.HasValue)
            RequiredLoadingDate = requiredLoadingDate.Value;

        if (requiredLoadingTime.HasValue)
            RequiredLoadingTime = requiredLoadingTime.Value;

        UpdateDestination(destination);
    }

    public void UpdateImportData(
        DateOnly? actualArrivalDate,
        TimeOnly? actualArrivalTime,
        DateOnly? gateOutDate,
        TimeOnly? gateOutTime,
        ContainerType? containerType,
        string? containerNumber,
        Guid? transporterId,
        Guid? vehicleId,
        Guid? driverId,
        DateTime? checkOutFromThePort,
        string? missingDocs,
        DateOnly? missingDocsReceivedDate,
        string? destination,
        DateOnly? freeTimeTill)
    {
        if (actualArrivalDate.HasValue)
            ActualArrivalDate = actualArrivalDate.Value;

        if (actualArrivalTime.HasValue)
            ActualArrivalTime = actualArrivalTime.Value;

        if (gateOutDate.HasValue)
            GateOutDate = gateOutDate.Value;

        if (gateOutTime.HasValue)
            GateOutTime = gateOutTime.Value;

        if (containerType.HasValue)
            ContainerType = containerType.Value;

        if (containerNumber is not null)
            ContainerNumber = containerNumber;

        if (transporterId.HasValue)
            TransporterId = transporterId.Value;

        if (vehicleId.HasValue)
            VehicleId = vehicleId.Value;

        if (driverId.HasValue)
            DriverId = driverId.Value;

        if (checkOutFromThePort.HasValue)
            CheckOutFromThePort = checkOutFromThePort.Value;

        if (missingDocs is not null)
            MissingDocs = missingDocs;

        if (missingDocsReceivedDate.HasValue)
            MissingDocsReceivedDate = missingDocsReceivedDate.Value;

        UpdateDestination(destination);

        if (freeTimeTill.HasValue)
            FreeTimeTill = freeTimeTill.Value;
    }

    public void InitializeImportData(
        string destination,
        DateOnly freeTimeTill)
    {
        UpdateDestination(destination);
        FreeTimeTill = freeTimeTill;
    }

    private void UpdateDestination(string? destination)
    {
        if (destination is null)
            return;

        var normalized = destination.Trim();

        if (normalized.Length == 0)
            throw new ArgumentException(
                "Destination cannot be empty.",
                nameof(destination));

        if (normalized.Length > 200)
            throw new ArgumentException(
                "Destination cannot exceed 200 characters.",
                nameof(destination));

        Destination = normalized;
    }
}
