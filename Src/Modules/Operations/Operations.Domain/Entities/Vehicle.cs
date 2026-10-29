using BuildingBlocks.Domain;

namespace Operations.Domain.Entities;

public class Vehicle : Entity<Guid>
{
    public string CarNumber { get; private set; } = null!;

    public bool IsActive { get; private set; } = true;

    private Vehicle() { }

    public static Vehicle Create(string carNumber)
    {
        if (string.IsNullOrWhiteSpace(carNumber))
            throw new ArgumentException("Car number cannot be empty.", nameof(carNumber));

        return new Vehicle
        {
            Id = Guid.NewGuid(),
            CarNumber = carNumber.Trim(),
            IsActive = true
        };
    }
}