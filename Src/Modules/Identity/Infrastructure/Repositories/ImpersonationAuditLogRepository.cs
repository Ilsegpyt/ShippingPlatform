using Identity.Application.Abstractions;
using Identity.Domain.Impersonation;
using Identity.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Identity.Infrastructure.Repositories;

public sealed class ImpersonationAuditLogRepository(
    IdentityDbContext dbContext)
    : IImpersonationAuditLogRepository
{
    public async Task AddAsync(
        ImpersonationAuditLog auditLog,
        CancellationToken ct)
    {
        await dbContext.ImpersonationAuditLogs.AddAsync(
            auditLog,
            ct);
    }

    public async Task<ImpersonationAuditLog?> GetByIdAsync(
        Guid id,
        CancellationToken ct)
    {
        return await dbContext.ImpersonationAuditLogs
            .FirstOrDefaultAsync(x => x.Id == id, ct);
    }
}