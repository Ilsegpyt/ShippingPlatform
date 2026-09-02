using BuildingBlocks.Application;
using BuildingBlocks.Application.Contracts;
using Identity.Application.Abstractions;
using Identity.Domain.Impersonation;
using MediatR;

namespace Identity.Application.Impersonation.ImpersonateCustomer;

public sealed class ImpersonateCustomerCommandHandler
    : IRequestHandler<
        ImpersonateCustomerCommand,
        Result<TokenPair>>
{
    private readonly ITokenService _tokens;
    private readonly ICustomerQueries _customers;
    private readonly IImpersonationAuditLogRepository _auditLogs;

    public ImpersonateCustomerCommandHandler(
        ITokenService tokens,
        ICustomerQueries customers,
        IImpersonationAuditLogRepository auditLogs)
    {
        _tokens = tokens;
        _customers = customers;
        _auditLogs = auditLogs;
    }

    public async Task<Result<TokenPair>> Handle(
        ImpersonateCustomerCommand request,
        CancellationToken ct)
    {
        var customer = await _customers.GetByOwnerUserIdAsync(
            request.TargetCustomerUserId,
            ct);

        if (customer is null)
        {
            return Result.Failure<TokenPair>(
                "Customer not found.");
        }

        if (!customer.IsActive)
        {
            return Result.Failure<TokenPair>(
                "Customer account is inactive.");
        }

        var claims = new Dictionary<string, string>
        {
            ["token_type"] = "impersonation",
            ["org_id"] = customer.CustomerId.ToString()
        };

        var auditLog = ImpersonationAuditLog.Start(
            request.ImpersonatorUserId,
            customer.CustomerId,
            request.IpAddress,
            request.UserAgent,
            request.Reason);

        await _auditLogs.AddAsync(auditLog, ct);

        var tokenPair = await _tokens.IssueTokensAsync(
            request.ImpersonatorUserId,
            claims,
            ct);

        return Result.Success(tokenPair);
    }
}