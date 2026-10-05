using BuildingBlocks.Domain;

namespace Operations.Domain.Entities;

public class Driver : Entity<Guid>
{
    public string DriverNumber { get; private set; } = null!;

    public string Name { get; private set; } = null!;

    public string? MobileNumber { get; private set; }

    public bool IsActive { get; private set; } = true;

    private Driver() { }
}