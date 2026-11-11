using BuildingBlocks.Domain;
using Operations.Domain.Common;
using Operations.Domain.Enums;

namespace Operations.Domain.Entities;

public class FtlTruck : Entity<Guid>
{
    public Guid FtlOperationId { get; private set; }
    public FtlOperation Operation { get; private set; } = null!;

    public short Ordinal { get; private set; }
    public Guid? TransporterId { get; private set; }
    public string? TransporterName { get; private set; }
    public FtlTruckType? TruckType { get; private set; }
    public string? TruckPlateNumber { get; private set; }
    public Guid? DriverId { get; private set; }
    public string? DriverName { get; private set; }
    public string? DriverMobile { get; private set; }

    public DateOnly? GateInDate { get; private set; }
    public TimeOnly? GateInTime { get; private set; }
    public DateOnly? GateOutDate { get; private set; }
    public TimeOnly? GateOutTime { get; private set; }

    public ICollection<FtlTruckUpdate> Updates { get; private set; } = new List<FtlTruckUpdate>();

    private FtlTruck() { }

    public FtlTruck(
        Guid ftlOperationId,
        short ordinal,
        Guid? transporterId = null,
        string? transporterName = null,
        FtlTruckType? truckType = null,
        string? truckPlateNumber = null,
        Guid? driverId = null,
        string? driverName = null,
        string? driverMobile = null,
        DateOnly? gateInDate = null,
        TimeOnly? gateInTime = null,
        DateOnly? gateOutDate = null,
        TimeOnly? gateOutTime = null)
        : base(Guid.NewGuid())
    {
        if (ftlOperationId == Guid.Empty)
            throw new ArgumentException("Operation is required.", nameof(ftlOperationId));

        FtlOperationId = ftlOperationId;
        Ordinal = ordinal;
        TransporterId = transporterId;
        TransporterName = string.IsNullOrWhiteSpace(transporterName) ? null : transporterName.Trim();
        TruckType = truckType;
        TruckPlateNumber = IdentifierNormalizer.Normalize(truckPlateNumber);
        DriverId = driverId;
        DriverName = string.IsNullOrWhiteSpace(driverName) ? null : driverName.Trim();
        DriverMobile = IdentifierNormalizer.Normalize(driverMobile);
        GateInDate = gateInDate;
        GateInTime = gateInTime;
        GateOutDate = gateOutDate;
        GateOutTime = gateOutTime;
    }

    public void Update(
        Guid? transporterId,
        string? transporterName,
        FtlTruckType? truckType,
        string? truckPlateNumber,
        Guid? driverId,
        string? driverName,
        string? driverMobile,
        DateOnly? gateInDate,
        TimeOnly? gateInTime,
        DateOnly? gateOutDate,
        TimeOnly? gateOutTime)
    {
        TransporterId = transporterId;
        TransporterName = string.IsNullOrWhiteSpace(transporterName) ? null : transporterName.Trim();
        TruckType = truckType;
        TruckPlateNumber = IdentifierNormalizer.Normalize(truckPlateNumber);
        DriverId = driverId;
        DriverName = string.IsNullOrWhiteSpace(driverName) ? null : driverName.Trim();
        DriverMobile = IdentifierNormalizer.Normalize(driverMobile);
        GateInDate = gateInDate;
        GateInTime = gateInTime;
        GateOutDate = gateOutDate;
        GateOutTime = gateOutTime;
    }

    public FtlTruckUpdate AddUpdate(string? tripId, string note, string userId, string userName, DateTime utcNow)
    {
        var update = new FtlTruckUpdate(Id, tripId, note, userId, userName, utcNow);
        Updates.Add(update);
        return update;
    }
}

