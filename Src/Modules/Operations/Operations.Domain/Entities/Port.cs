using BuildingBlocks.Domain;

namespace Operations.Domain.Entities;

public class Port : Entity<Guid>
{
    public string Name { get; private set; } = null!;

    public string? Code { get; private set; }

    public bool IsActive { get; private set; } = true;

    private Port() { }
}