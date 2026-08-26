namespace Identity.Infrastructure.Persistence;

/// <summary>
/// Audit record for an impersonation session.
/// Records who impersonated which customer and when the session started and ended.
/// </summary>
public sealed class ImpersonationAuditLog
{
    public Guid Id { get; private set; }

    public Guid ImpersonatorUserId { get; private set; }

    public Guid TargetCustomerUserId { get; private set; }

    public DateTime StartedAtUtc { get; private set; }

    public DateTime? EndedAtUtc { get; private set; }

    private ImpersonationAuditLog() { }

    public static ImpersonationAuditLog Start(
        Guid impersonatorUserId,
        Guid targetCustomerUserId)
    {
        return new ImpersonationAuditLog
        {
            Id = Guid.NewGuid(),
            ImpersonatorUserId = impersonatorUserId,
            TargetCustomerUserId = targetCustomerUserId,
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