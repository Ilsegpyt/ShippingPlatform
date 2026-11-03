using BuildingBlocks.Domain;

namespace Operations.Domain.Entities;

public class ShippingLine : Entity<Guid>, ISoftDeletable
{
    public string Name { get; private set; } = null!;
    public bool IsActive { get; private set; } = true;

    public bool IsDeleted { get; private set; }
    public DateTime? DeletedAtUtc { get; private set; }
    public Guid? DeletedByUserId { get; private set; }
    public string? DeletedByName { get; private set; }

    private ShippingLine() { }

    public static ShippingLine Create(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Shipping line name cannot be empty.", nameof(name));

        return new ShippingLine
        {
            Id = Guid.NewGuid(),
            Name = name.Trim(),
            IsActive = true
        };
    }

    public void Update(string name, bool isActive)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Shipping line name cannot be empty.", nameof(name));

        Name = name.Trim();
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
        DeletedAtUtc = utcNow;
    }
}
