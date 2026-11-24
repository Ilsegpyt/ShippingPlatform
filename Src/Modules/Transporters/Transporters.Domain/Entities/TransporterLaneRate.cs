namespace Transporters.Domain.Entities;

public class TransporterLaneRate
{
    public Guid Id { get; set; }

    public Guid PortCoverageId { get; set; }

    public string Destination { get; set; } = null!;
    public string EquipmentType { get; set; } = null!;

    public decimal Price { get; set; }

    public string? Notes { get; set; }
}
