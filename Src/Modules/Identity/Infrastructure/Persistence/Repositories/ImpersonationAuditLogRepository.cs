using Identity.Application.Abstractions;
using Identity.Domain.Impersonation;
using Identity.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Identity.Infrastructure.Persistence.Repositories;

public sealed class ImpersonationAuditLogRepository(IdentityDbContext dbContext) : IImpersonationAuditLogRepository
{
  
    public void Add(ImpersonationAuditLog auditLog, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public async Task<ImpersonationAuditLog?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        return await dbContext.ImpersonationAuditLogs.FirstOrDefaultAsync(x => x.Id == id, ct);

    }
}