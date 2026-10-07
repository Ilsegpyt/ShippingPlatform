namespace Website.Domain.Entities;

public class ContactInquiry
{
    public int Id { get; set; }

    public string FullName { get; set; } = null!;
    public string Company { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Phone { get; set; } = null!;
    public string Service { get; set; } = null!;
    public string Message { get; set; } = null!;

    public DateTime CreatedAtUtc { get; set; }
}