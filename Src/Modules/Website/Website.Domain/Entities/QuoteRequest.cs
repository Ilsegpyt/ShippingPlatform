using Website.Domain.Enums;

namespace Website.Domain.Entities;

public class QuoteRequest
{
    public int Id { get; set; }

    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;

    public string Company { get; set; } = null!;
    public string Country { get; set; } = null!;

    public string CountryCode { get; set; } = null!;
    public string Phone { get; set; } = null!;

    public string Email { get; set; } = null!;
    public string Message { get; set; } = null!;

    public InterestType InterestType { get; set; }

    public List<TransportMode> TransportModes { get; set; } = [];

    public AnnualShipments AnnualShipments { get; set; }

    public DateTime CreatedAtUtc { get; set; }
}