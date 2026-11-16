using BuildingBlocks.Domain;
using Operations.Domain.Enums;

namespace Operations.Domain.Entities;

public class FtlVehicle : Entity<Guid>, ISoftDeletable
{
    public string PlateNumber { get; private set; } = null!;
    public FtlTruckType TruckType { get; private set; }
    public Guid? FtlTransporterId { get; private set; }
    public FtlTransporter? Transporter { get; private set; }
    public bool IsActive { get; private set; } = true;

    public bool IsDeleted { get; private set; }
    public DateTime? DeletedAtUtc { get; private set; }
    public Guid? DeletedByUserId { get; private set; }
    public string? DeletedByName { get; private set; }

    private FtlVehicle() { }

    public static FtlVehicle Create(string plateNumber, FtlTruckType truckType, Guid? ftlTransporterId = null)
    {
        if (string.IsNullOrWhiteSpace(plateNumber))
            throw new ArgumentException("Plate number cannot be empty.", nameof(plateNumber));

        return new FtlVehicle
        {
            Id = Guid.NewGuid(),
            PlateNumber = plateNumber.Trim(),
            TruckType = truckType,
            FtlTransporterId = ftlTransporterId,
            IsActive = true
        };
    }

    public void Update(string plateNumber, FtlTruckType truckType, Guid? ftlTransporterId, bool isActive)
    {
        if (string.IsNullOrWhiteSpace(plateNumber))
            throw new ArgumentException("Plate number cannot be empty.", nameof(plateNumber));

        PlateNumber = plateNumber.Trim();
        TruckType = truckType;
        FtlTransporterId = ftlTransporterId;
        IsActive = isActive;

        if (isActive)
        {
            IsDeleted = false;
            DeletedAtUtc = null;
            DeletedByUserId = null;
            DeletedByName = null;
        }
    }

    public void MarkAsDeleted(Guid deletedByUserId, string deletedByName, DateTime utcNow)
    {
        IsDeleted = true;
        IsActive = false;
        DeletedByUserId = deletedByUserId;
        DeletedByName = deletedByName;
        DeletedAtUtc = utcNow.Kind == DateTimeKind.Utc ? utcNow : DateTime.SpecifyKind(utcNow, DateTimeKind.Utc);
    }
}

