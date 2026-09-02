namespace Identity.Domain.Impersonation;

public sealed class ImpersonationAuditLog
{
    public Guid Id { get; private set; }

    public Guid ImpersonatorUserId { get; private set; }

    public Guid TargetCustomerUserId { get; private set; }

    public string? IpAddress { get; private set; }

    public string? UserAgent { get; private set; }

    public string? Reason { get; private set; }

    public DateTime StartedAtUtc { get; private set; }

    public DateTime? EndedAtUtc { get; private set; }

    private ImpersonationAuditLog()
    {
    }

    public static ImpersonationAuditLog Start(
        Guid impersonatorUserId,
        Guid targetCustomerUserId,
        string? ipAddress,
        string? userAgent,
        string? reason)
    {
        return new ImpersonationAuditLog
        {
            Id = Guid.NewGuid(),
            ImpersonatorUserId = impersonatorUserId,
            TargetCustomerUserId = targetCustomerUserId,
            IpAddress = ipAddress,
            UserAgent = userAgent,
            Reason = reason,
            StartedAtUtc = DateTime.UtcNow
        };
    }

    public void End()
    {
        if (EndedAtUtc is not null)
            return;

        EndedAtUtc = DateTime.UtcNow;
    }
}