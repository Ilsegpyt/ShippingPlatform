namespace Website.Domain.Entities;

public class RecruitmentApplication
{
    public int Id { get; set; }

    public string Department { get; set; } = null!;

    public string CvFileName { get; set; } = null!;
    public string CvFilePath { get; set; } = null!;

    public string Message { get; set; } = null!;

    public DateTime CreatedAtUtc { get; set; }
}