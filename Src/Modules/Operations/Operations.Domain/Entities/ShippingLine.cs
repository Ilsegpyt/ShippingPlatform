using BuildingBlocks.Domain;

namespace Operations.Domain.Entities;

public class ShippingLine : SoftDeletableEntity<Guid>
{
    public Guid Id { get; private set; } = Guid.NewGuid();

    public string Name { get; private set; } = null!;

    public bool IsActive { get; private set; } = true;

    private ShippingLine() { }
}