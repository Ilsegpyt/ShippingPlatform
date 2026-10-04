using BuildingBlocks.Domain;

namespace Operations.Domain.Entities;

public class Vehicle : SoftDeletableEntity<Guid>
{
    public Guid Id { get; private set; } = Guid.NewGuid();

    public string CarNumber { get; private set; } = null!;

    public bool IsActive { get; private set; } = true;

    private Vehicle() { }
}