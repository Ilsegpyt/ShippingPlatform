using BuildingBlocks.Application;
using MediatR;

namespace Identity.Application.Impersonation.EndImpersonation;

public sealed record EndImpersonationCommand(
    Guid AuditLogId,
    Guid ImpersonatorUserId)
    : IRequest<Result>;