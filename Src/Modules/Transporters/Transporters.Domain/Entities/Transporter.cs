namespace Transporters.Domain.Entities;

public class Transporter
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string? ContactPerson { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAtUtc { get; set; }

    public List<TransporterPortCoverage> PortCoverages { get; set; } = new();
    public List<TransporterClientException> ClientExceptions { get; set; } = new();
}
