using BuildingBlocks.Domain;

namespace Operations.Domain.Entities;

public class FtlAirport : Entity<Guid>, ISoftDeletable
{
    public string Name { get; private set; } = null!;
    public string? Code { get; private set; }
    public bool IsActive { get; private set; } = true;

    public bool IsDeleted { get; private set; }
    public DateTime? DeletedAtUtc { get; private set; }
    public Guid? DeletedByUserId { get; private set; }
    public string? DeletedByName { get; private set; }

    private FtlAirport() { }

    public static FtlAirport Create(string name, string? code = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Airport name cannot be empty.", nameof(name));

        return new FtlAirport
        {
            Id = Guid.NewGuid(),
            Name = name.Trim(),
            Code = string.IsNullOrWhiteSpace(code) ? null : code.Trim().ToUpperInvariant(),
            IsActive = true
        };
    }

    public void Update(string name, string? code, bool isActive)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Airport name cannot be empty.", nameof(name));

        Name = name.Trim();
        Code = string.IsNullOrWhiteSpace(code) ? null : code.Trim().ToUpperInvariant();
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

