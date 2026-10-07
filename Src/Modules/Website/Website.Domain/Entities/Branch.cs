namespace Website.Domain.Entities;

public class Branch
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;
    public string Address { get; set; } = null!;

    public decimal Latitude { get; set; }
    public decimal Longitude { get; set; }

    public string? CeoName { get; set; }
    public string? CeoEmail { get; set; }

    public string? GmName { get; set; }
    public string? GmEmail { get; set; }

    public string? BranchManagerName { get; set; }
    public string? BranchManagerEmail { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAtUtc { get; set; }
}