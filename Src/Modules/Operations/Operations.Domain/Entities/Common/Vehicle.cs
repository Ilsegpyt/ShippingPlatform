using BuildingBlocks.Domain;

namespace Operations.Domain.Entities;

public class Vehicle : Entity<Guid>, ISoftDeletable
{
    public string CarNumber { get; private set; } = null!;
    public bool IsActive { get; private set; } = true;

    public bool IsDeleted { get; private set; }
    public DateTime? DeletedAtUtc { get; private set; }
    public Guid? DeletedByUserId { get; private set; }
    public string? DeletedByName { get; private set; }

    private Vehicle() { }

    public static Vehicle Create(string carNumber)
    {
        if (string.IsNullOrWhiteSpace(carNumber))
            throw new ArgumentException("Car number cannot be empty.", nameof(carNumber));

        return new Vehicle
        {
            Id = Guid.NewGuid(),
            CarNumber = carNumber.Trim(),
            IsActive = true
        };
    }

    public void Update(string carNumber, bool isActive)
    {
        if (string.IsNullOrWhiteSpace(carNumber))
            throw new ArgumentException("Car number cannot be empty.", nameof(carNumber));

        CarNumber = carNumber.Trim();
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
