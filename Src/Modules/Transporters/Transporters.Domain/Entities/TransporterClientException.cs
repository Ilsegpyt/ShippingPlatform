namespace Transporters.Domain.Entities;

public class TransporterClientException
{
    public Guid Id { get; set; }

    public Guid TransporterId { get; set; }

    public Guid CustomerId { get; set; }

    public Guid PortCoverageId { get; set; }

    public string Destination { get; set; } = null!;
    public string EquipmentType { get; set; } = null!;

    public decimal AgreedPrice { get; set; }

    public DateTime ValidFromUtc { get; set; }
    public DateTime ValidToUtc { get; set; }

    public string? Reason { get; set; }

    public bool IsActive { get; set; } = true;
}
