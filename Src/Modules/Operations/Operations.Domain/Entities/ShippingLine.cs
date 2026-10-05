using BuildingBlocks.Domain;

namespace Operations.Domain.Entities;

public class ShippingLine : Entity<Guid>
{
    public string Name { get; private set; } = null!;

    public bool IsActive { get; private set; } = true;

    private ShippingLine() { }
}