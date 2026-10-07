namespace Website.Domain.Entities;

public class Branch
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;
    public string Address { get; set; } = null!;

    public decimal Latitude { get; set; }
    public decimal Longitude { get; set; }

    public string CeoName { get; set; } = null!;
    public string CeoEmail { get; set; } = null!;

    public string GmName { get; set; } = null!;
    public string GmEmail { get; set; } = null!;

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAtUtc { get; set; }
}