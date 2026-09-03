namespace Shipments.Domain.Declarations;

public sealed class DeclarationFile
{
    public Guid Id { get; private set; }

    public Guid ShipmentId { get; private set; }

    public string FileName { get; private set; } = null!;

    public string StorageKey { get; private set; } = null!;

    public DateTime UploadedAtUtc { get; private set; }

    private DeclarationFile()
    {
    }

    public static DeclarationFile Create(
        Guid shipmentId,
        string fileName,
        string storageKey)
    {
        return new DeclarationFile
        {
            Id = Guid.NewGuid(),
            ShipmentId = shipmentId,
            FileName = fileName,
            StorageKey = storageKey,
            UploadedAtUtc = DateTime.UtcNow
        };
    }
}