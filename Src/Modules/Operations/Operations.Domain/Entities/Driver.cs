using BuildingBlocks.Domain;

namespace Operations.Domain.Entities;

public class Driver : SoftDeletableEntity<Guid>
{ 
    public Guid Id { get; private set; } = Guid.NewGuid();

    public string DriverNumber { get; private set; } = null!;

    public string Name { get; private set; } = null!;

    public string? MobileNumber { get; private set; }

    public bool IsActive { get; private set; } = true;

    private Driver() { }
}