using BuildingBlocks.Domain;
using Operations.Domain.Enums;

namespace Operations.Domain.Entities;

public class OperationContainer : SoftDeletableEntity<Guid>
{
    public Guid OperationId { get; private set; }

    public short Ordinal { get; private set; }

    public string? ContainerNumber { get; private set; }

    public ContainerType? ContainerType { get; private set; }

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

    public OperationContainer(
        Guid operationId,
        short ordinal)
    {
        OperationId = operationId;
        Ordinal = ordinal;
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
        DateOnly? missingDocsReceivedDate)
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
    }
}