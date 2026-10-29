using BuildingBlocks.Domain;

namespace Operations.Domain.Entities;

public class Destination : Entity<Guid>
{
    public string Name { get; private set; } = null!;

    public bool IsActive { get; private set; } = true;

    private Destination() { }

    public static Destination Create(string name)
    {
        return new Destination
        {
            Id = Guid.NewGuid(),
            Name = name.Trim(),
            IsActive = true
        };
    }
}
