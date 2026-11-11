using BuildingBlocks.Domain;

namespace Operations.Domain.Entities;

public class FtlDriver : Entity<Guid>, ISoftDeletable
{
    public string Name { get; private set; } = null!;
    public string? DriverNumber { get; private set; }
    public string? MobileNumber { get; private set; }
    public bool IsActive { get; private set; } = true;

    public bool IsDeleted { get; private set; }
    public DateTime? DeletedAtUtc { get; private set; }
    public Guid? DeletedByUserId { get; private set; }
    public string? DeletedByName { get; private set; }

    private FtlDriver() { }

    public static FtlDriver Create(string name, string? mobileNumber = null, string? driverNumber = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Driver name cannot be empty.", nameof(name));

        return new FtlDriver
        {
            Id = Guid.NewGuid(),
            Name = name.Trim(),
            MobileNumber = string.IsNullOrWhiteSpace(mobileNumber) ? null : mobileNumber.Trim(),
            DriverNumber = string.IsNullOrWhiteSpace(driverNumber) ? null : driverNumber.Trim(),
            IsActive = true
        };
    }

    public void Update(string name, string? mobileNumber, string? driverNumber, bool isActive)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Driver name cannot be empty.", nameof(name));

        Name = name.Trim();
        MobileNumber = string.IsNullOrWhiteSpace(mobileNumber) ? null : mobileNumber.Trim();
        DriverNumber = string.IsNullOrWhiteSpace(driverNumber) ? null : driverNumber.Trim();
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

