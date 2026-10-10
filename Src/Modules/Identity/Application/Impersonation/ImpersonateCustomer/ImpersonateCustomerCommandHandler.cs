
using BuildingBlocks.Application;
using Customers.Contracts;
using Identity.Application.Abstractions;
using Identity.Domain.Impersonation;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Identity.Application.Impersonation.ImpersonateCustomer;

public sealed class ImpersonateCustomerCommandHandler
    : IRequestHandler<
        ImpersonateCustomerCommand,
        Result<ImpersonateCustomerResponse>>
{
    private readonly ITokenService _tokens;
    private readonly ICustomerQueries _customers;
    private readonly IImpersonationAuditLogRepository _auditLogs;
    private readonly IIdentityUnitOfWork _identityUnitOfWork;
    private readonly ILogger<ImpersonateCustomerCommandHandler> _logger;

    public ImpersonateCustomerCommandHandler(
        ITokenService tokens,
        ICustomerQueries customers,
        IImpersonationAuditLogRepository auditLogs,
        IIdentityUnitOfWork unitOfWork,
        ILogger<ImpersonateCustomerCommandHandler> logger)
    {
        _tokens = tokens;
        _customers = customers;
        _auditLogs = auditLogs;
        _identityUnitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<ImpersonateCustomerResponse>> Handle(
        ImpersonateCustomerCommand request,
        CancellationToken ct)
    {
        var customer = await _customers.GetByUserIdAsync(
            request.TargetCustomerUserId,
            ct);

        if (customer is null)
        {
            return Result.Failure<ImpersonateCustomerResponse>(
                "Customer not found.");
        }

        if (!customer.IsActive)
        {
            return Result.Failure<ImpersonateCustomerResponse>(
                "Customer account is inactive.");
        }

        var auditLog = ImpersonationAuditLog.Start(
            request.ImpersonatorUserId,
            customer.CustomerId,
            request.IpAddress,
            request.UserAgent,
            request.Reason);

        _auditLogs.Add(auditLog, ct);

        var tokenPair = await _tokens.IssueImpersonationTokensAsync(
            request.ImpersonatorUserId,
            customer.CustomerId,
            ct);

        await _identityUnitOfWork.SaveChangesAsync(ct);

        _logger.LogInformation(
            "User {ImpersonatorUserId} started impersonation for customer {CustomerId}. AuditLogId: {AuditLogId}",
            request.ImpersonatorUserId,
            customer.CustomerId,
            auditLog.Id);

        var response = new ImpersonateCustomerResponse(
            tokenPair.AccessToken,
            tokenPair.RefreshToken,
            tokenPair.AccessTokenExpiresAtUtc,
            auditLog.Id);

        return Result.Success(response);
    }
}
