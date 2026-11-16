using BuildingBlocks.Domain;
using Operations.Domain.Enums;

namespace Operations.Domain.Entities;

public class FtlOperationTrip : Entity<Guid>
{
    public Guid FtlOperationId { get; private set; }
    public FtlOperation Operation { get; private set; } = null!;

    public short Ordinal { get; private set; }
    public string? ExternalId { get; private set; }

    public Guid? FtlShipmentId { get; private set; }
    public FtlShipment? Shipment { get; private set; }

    public Guid? FtlTruckId { get; private set; }
    public FtlTruck? Truck { get; private set; }

    public FtlLoadStatus LoadStatus { get; private set; } = FtlLoadStatus.Full;
    public int? LoadedPackages { get; private set; }
    public decimal? LoadedWeight { get; private set; }
    public decimal? LoadedCbm { get; private set; }

    /// <summary>
    /// Raw JSON representation of row-level UI fields for 100% fidelity with ftlEngine.js
    /// </summary>
    public string? FieldsJson { get; private set; }

    private FtlOperationTrip() { }

    public FtlOperationTrip(
        Guid id,
        Guid ftlOperationId,
        short ordinal,
        string? externalId = null,
        Guid? ftlShipmentId = null,
        Guid? ftlTruckId = null,
        FtlLoadStatus loadStatus = FtlLoadStatus.Full,
        int? loadedPackages = null,
        decimal? loadedWeight = null,
        decimal? loadedCbm = null,
        string? fieldsJson = null)
        : base(id == Guid.Empty ? Guid.NewGuid() : id)
    {
        if (ftlOperationId == Guid.Empty)
            throw new ArgumentException("Operation is required.", nameof(ftlOperationId));

        FtlOperationId = ftlOperationId;
        Ordinal = ordinal;
        ExternalId = string.IsNullOrWhiteSpace(externalId) ? null : externalId.Trim();
        FtlShipmentId = ftlShipmentId;
        FtlTruckId = ftlTruckId;
        LoadStatus = loadStatus;
        LoadedPackages = loadedPackages;
        LoadedWeight = loadedWeight;
        LoadedCbm = loadedCbm;
        FieldsJson = fieldsJson;
    }

    public void Update(
        short ordinal,
        string? externalId,
        Guid? ftlShipmentId,
        Guid? ftlTruckId,
        FtlLoadStatus loadStatus,
        int? loadedPackages,
        decimal? loadedWeight,
        decimal? loadedCbm,
        string? fieldsJson)
    {
        Ordinal = ordinal;
        ExternalId = string.IsNullOrWhiteSpace(externalId) ? null : externalId.Trim();
        FtlShipmentId = ftlShipmentId;
        FtlTruckId = ftlTruckId;
        LoadStatus = loadStatus;
        LoadedPackages = loadedPackages;
        LoadedWeight = loadedWeight;
        LoadedCbm = loadedCbm;
        FieldsJson = fieldsJson;
    }
}

