using Identity.Domain.Impersonation;

namespace Identity.Application.Abstractions;

public interface IImpersonationAuditLogRepository
{
    void Add(ImpersonationAuditLog auditLog, CancellationToken ct);

    Task<ImpersonationAuditLog?> GetByIdAsync(Guid id, CancellationToken ct);

}