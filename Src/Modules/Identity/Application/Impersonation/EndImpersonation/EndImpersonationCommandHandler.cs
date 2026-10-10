
using BuildingBlocks.Application;
using Identity.Application.Abstractions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Identity.Application.Impersonation.EndImpersonation;

public sealed class EndImpersonationCommandHandler
    : IRequestHandler<EndImpersonationCommand, Result>
{
    private readonly IImpersonationAuditLogRepository _auditLogs;
    private readonly IIdentityUnitOfWork _identityUnitOfWork;
    private readonly ILogger<EndImpersonationCommandHandler> _logger;

    public EndImpersonationCommandHandler(
        IImpersonationAuditLogRepository auditLogs,
        IIdentityUnitOfWork unitOfWork,
        ILogger<EndImpersonationCommandHandler> logger)
    {
        _auditLogs = auditLogs;
        _identityUnitOfWork = unitOfWork;
        _logger = logger;
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
            _logger.LogWarning(
                "User {ImpersonatorUserId} attempted to end an impersonation session owned by another user. AuditLogId: {AuditLogId}",
                request.ImpersonatorUserId,
                request.AuditLogId);

            return Result.Failure(
                "You are not allowed to end this impersonation session.");
        }

        if (auditLog.EndedAtUtc is not null)
        {
            return Result.Failure(
                "Impersonation session has already ended.");
        }

        auditLog.End();

        await _identityUnitOfWork.SaveChangesAsync(ct);

        _logger.LogInformation(
            "User {ImpersonatorUserId} ended impersonation successfully. AuditLogId: {AuditLogId}",
            request.ImpersonatorUserId,
            request.AuditLogId);

        return Result.Success();
    }
}
