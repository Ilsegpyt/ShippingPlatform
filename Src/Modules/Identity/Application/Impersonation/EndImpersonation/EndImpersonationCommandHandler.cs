using BuildingBlocks.Application;
using Identity.Application.Abstractions;
using MediatR;

namespace Identity.Application.Impersonation.EndImpersonation;

public sealed class EndImpersonationCommandHandler
    : IRequestHandler<EndImpersonationCommand, Result>
{
    private readonly IImpersonationAuditLogRepository _auditLogs;
    private readonly IIdentityUnitOfWork _unitOfWork;

    public EndImpersonationCommandHandler(
        IImpersonationAuditLogRepository auditLogs,
        IIdentityUnitOfWork unitOfWork)
    {
        _auditLogs = auditLogs;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(
        EndImpersonationCommand request,
        CancellationToken ct)
    {
        var auditLog = await _auditLogs.GetByIdAsync(
            request.AuditLogId,
            ct);

        if (auditLog is null)
        {
            return Result.Failure(
                "Impersonation session not found.");
        }

        if (auditLog.ImpersonatorUserId != request.ImpersonatorUserId)
        {
            return Result.Failure(
                "You are not allowed to end this impersonation session.");
        }

        if (auditLog.EndedAtUtc is not null)
        {
            return Result.Failure(
                "Impersonation session has already ended.");
        }

        auditLog.End();

        await _unitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }
}