namespace Shipments.Domain.Shipments;

public sealed class Shipment
{
    public Guid Id { get; private set; }

    public string ShipmentRef { get; private set; } = null!;

    public Guid CustomerId { get; private set; }

    public Guid ScheduleId { get; private set; }

    public string Mode { get; private set; } = null!;

    public string Carrier { get; private set; } = null!;

    public string ContainerType { get; private set; } = null!;

    public int Quantity { get; private set; }

    public decimal Rate { get; private set; }

    public decimal Total { get; private set; }

    public ShipmentStatus Status { get; private set; }

    public string? MBL { get; private set; }

    public string? HBL { get; private set; }

    public string? MAWB { get; private set; }

    public string? BookingConfirmationNumber { get; private set; }

    public DateTime CreatedAtUtc { get; private set; }

    private Shipment()
    {
    }
    public void Update(
    ShipmentStatus status,
    string? mbl,
    string? hbl,
    string? mawb,
    string? bookingConfirmationNumber)
    {
        Status = status;
        MBL = mbl;
        HBL = hbl;
        MAWB = mawb;
        BookingConfirmationNumber = bookingConfirmationNumber;
    }
    public static Shipment Create(
        Guid customerId,
        Guid scheduleId,
        string mode,
        string carrier,
        string containerType,
        int quantity,
        decimal rate)
    {
        if (quantity <= 0)
            throw new ArgumentOutOfRangeException(
                nameof(quantity),
                "Quantity must be greater than zero.");

        if (rate < 0)
            throw new ArgumentOutOfRangeException(
                nameof(rate),
                "Rate cannot be negative.");

        return new Shipment
        {
            Id = Guid.NewGuid(),
            ShipmentRef = GenerateShipmentReference(),
            CustomerId = customerId,
            ScheduleId = scheduleId,
            Mode = mode,
            Carrier = carrier,
            ContainerType = containerType,
            Quantity = quantity,
            Rate = rate,
            Total = quantity * rate,
            Status = ShipmentStatus.Received,
            CreatedAtUtc = DateTime.UtcNow
        };
    }

    private static string GenerateShipmentReference()
    {
        return $"BOOK-{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}";
    }
}

public enum ShipmentStatus
{
    Received = 1,
    Processing = 2,
    MissingDocs = 3,
    Confirmed = 4,
    Delivered = 5,
    Cancelled = 6
}