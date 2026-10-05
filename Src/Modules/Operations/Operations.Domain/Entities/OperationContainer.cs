using BuildingBlocks.Domain;
using Operations.Domain.Enums;

namespace Operations.Domain.Entities;

public class OperationContainer : SoftDeletableEntity<Guid>
{
    public Guid OperationId { get; private set; }

    public short Ordinal { get; private set; }

    public string? ContainerNumber { get; private set; }

    public ContainerType ContainerType { get; private set; }

    public DateOnly? RequiredLoadingDate { get; private set; }

    public TimeOnly? RequiredLoadingTime { get; private set; }

    public DateOnly? ActualArrivalDate { get; private set; }

    public TimeOnly? ActualArrivalTime { get; private set; }

    public DateOnly? GateOutDate { get; private set; }

    public TimeOnly? GateOutTime { get; private set; }

    public string? MissingDocs { get; private set; }

    public DateOnly? MissingDocsReceivedDate { get; private set; }

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
}