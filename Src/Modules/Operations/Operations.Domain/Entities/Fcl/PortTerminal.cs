using BuildingBlocks.Domain;

namespace Operations.Domain.Entities.Fcl;

public class PortTerminal : Entity<Guid>, ISoftDeletable
{
    public Guid PortId { get; private set; }
    public Port Port { get; private set; } = null!;
    public string Name { get; private set; } = null!;
    public string? Code { get; private set; }
    public string? ArabicName { get; private set; }
    public bool IsActive { get; private set; } = true;

    public bool IsDeleted { get; private set; }
    public DateTime? DeletedAtUtc { get; private set; }
    public Guid? DeletedByUserId { get; private set; }
    public string? DeletedByName { get; private set; }

    private PortTerminal() { }

    public static PortTerminal Create(Guid portId, string name, string? code = null, string? arabicName = null)
    {
        if (portId == Guid.Empty)
            throw new ArgumentException("Port ID cannot be empty.", nameof(portId));

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Terminal name cannot be empty.", nameof(name));

        return new PortTerminal
        {
            Id = Guid.NewGuid(),
            PortId = portId,
            Name = name.Trim(),
            Code = string.IsNullOrWhiteSpace(code) ? null : code.Trim().ToUpperInvariant(),
            ArabicName = string.IsNullOrWhiteSpace(arabicName) ? null : arabicName.Trim(),
            IsActive = true
        };
    }

    public void Update(Guid portId, string name, string? code, string? arabicName, bool isActive)
    {
        if (portId == Guid.Empty)
            throw new ArgumentException("Port ID cannot be empty.", nameof(portId));

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Terminal name cannot be empty.", nameof(name));

        PortId = portId;
        Name = name.Trim();
        Code = string.IsNullOrWhiteSpace(code) ? null : code.Trim().ToUpperInvariant();
        ArabicName = string.IsNullOrWhiteSpace(arabicName) ? null : arabicName.Trim();
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
