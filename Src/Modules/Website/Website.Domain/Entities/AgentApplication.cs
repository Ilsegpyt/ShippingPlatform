namespace Website.Domain.Entities;

public class AgentApplication
{
    public int Id { get; set; }

    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;

    public string Country { get; set; } = null!;
    public string City { get; set; } = null!;
    public string Address { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string CountryCode { get; set; } = null!;
    public string Phone { get; set; } = null!;

    public string MainIndustry { get; set; } = null!;
    public string Message { get; set; } = null!;

    public DateTime CreatedAtUtc { get; set; }
}