using BuildingBlocks.Domain;

namespace Operations.Domain.Entities;

public class Driver : Entity<Guid>, ISoftDeletable
{
    public string DriverNumber { get; private set; } = null!;
    public string Name { get; private set; } = null!;
    public string? MobileNumber { get; private set; }
    public bool IsActive { get; private set; } = true;

    public bool IsDeleted { get; private set; }
    public DateTime? DeletedAtUtc { get; private set; }
    public Guid? DeletedByUserId { get; private set; }
    public string? DeletedByName { get; private set; }

    private Driver() { }

    public static Driver Create(string driverNumber, string name, string? mobileNumber = null)
    {
        if (string.IsNullOrWhiteSpace(driverNumber))
            throw new ArgumentException("Driver number cannot be empty.", nameof(driverNumber));
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Driver name cannot be empty.", nameof(name));

        return new Driver
        {
            Id = Guid.NewGuid(),
            DriverNumber = driverNumber.Trim(),
            Name = name.Trim(),
            MobileNumber = string.IsNullOrWhiteSpace(mobileNumber) ? null : mobileNumber.Trim(),
            IsActive = true
        };
    }

    public void Update(string driverNumber, string name, string? mobileNumber, bool isActive)
    {
        if (string.IsNullOrWhiteSpace(driverNumber))
            throw new ArgumentException("Driver number cannot be empty.", nameof(driverNumber));
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Driver name cannot be empty.", nameof(name));

        DriverNumber = driverNumber.Trim();
        Name = name.Trim();
        MobileNumber = string.IsNullOrWhiteSpace(mobileNumber) ? null : mobileNumber.Trim();
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
