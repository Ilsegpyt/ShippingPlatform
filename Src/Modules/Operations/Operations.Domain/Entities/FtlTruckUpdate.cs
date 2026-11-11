using BuildingBlocks.Domain;

namespace Operations.Domain.Entities;

public class FtlTruckUpdate : Entity<Guid>
{
    public Guid FtlTruckId { get; private set; }
    public FtlTruck Truck { get; private set; } = null!;

    public string? TripId { get; private set; }
    public string Note { get; private set; } = null!;

    public DateTime CreatedAtUtc { get; private set; }
    public string CreatedByUserId { get; private set; } = null!;
    public string CreatedByName { get; private set; } = null!;

    private FtlTruckUpdate() { }

    public FtlTruckUpdate(Guid ftlTruckId, string? tripId, string note, string userId, string userName, DateTime utcNow)
        : base(Guid.NewGuid())
    {
        if (ftlTruckId == Guid.Empty) throw new ArgumentException("Truck is required.", nameof(ftlTruckId));
        if (string.IsNullOrWhiteSpace(note)) throw new ArgumentException("Note cannot be empty.", nameof(note));
        if (string.IsNullOrWhiteSpace(userId)) throw new ArgumentException("User ID is required.", nameof(userId));

        FtlTruckId = ftlTruckId;
        TripId = string.IsNullOrWhiteSpace(tripId) ? null : tripId.Trim();
        Note = note.Trim();
        CreatedByUserId = userId.Trim();
        CreatedByName = string.IsNullOrWhiteSpace(userName) ? "User" : userName.Trim();
        CreatedAtUtc = utcNow.Kind == DateTimeKind.Utc ? utcNow : DateTime.SpecifyKind(utcNow, DateTimeKind.Utc);
    }
}

