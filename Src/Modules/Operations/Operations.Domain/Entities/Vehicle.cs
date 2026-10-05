using BuildingBlocks.Domain;

namespace Operations.Domain.Entities;

public class Vehicle : Entity<Guid>
{
    public string CarNumber { get; private set; } = null!;

    public bool IsActive { get; private set; } = true;

    private Vehicle() { }
}