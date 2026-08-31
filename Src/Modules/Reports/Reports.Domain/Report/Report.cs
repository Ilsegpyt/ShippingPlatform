using BuildingBlocks.Domain;

namespace Reports.Domain.Report;

public sealed class Report : Entity<Guid>
{
    public Guid CustomerId { get; private set; }

    public string? ShipmentRef { get; private set; }

    public ReportCategory Category { get; private set; }

    public ReportService Service { get; private set; }

    public ReportShipmentType ShipmentType { get; private set; }

    public string FileName { get; private set; } = null!;

    public string StorageKey { get; private set; } = null!;

    public Guid UploadedByUserId { get; private set; }

    public DateTime UploadedAtUtc { get; private set; }

    private Report() { }

    private Report(
        Guid id,
        Guid customerId,
        string? shipmentRef,
        ReportCategory category,
        ReportService service,
        ReportShipmentType shipmentType,
        string fileName,
        string storageKey,
        Guid uploadedByUserId)
        : base(id)
    {
        CustomerId = customerId;
        ShipmentRef = shipmentRef;
        Category = category;
        Service = service;
        ShipmentType = shipmentType;
        FileName = fileName;
        StorageKey = storageKey;
        UploadedByUserId = uploadedByUserId;
        UploadedAtUtc = DateTime.UtcNow;
    }

    public static Report Create(
        Guid customerId,
        string? shipmentRef,
        ReportCategory category,
        ReportService service,
        ReportShipmentType shipmentType,
        string fileName,
        string storageKey,
        Guid uploadedByUserId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(fileName);
        ArgumentException.ThrowIfNullOrWhiteSpace(storageKey);

        return new Report(
            Guid.NewGuid(),
            customerId,
            shipmentRef,
            category,
            service,
            shipmentType,
            fileName,
            storageKey,
            uploadedByUserId);
    }
}