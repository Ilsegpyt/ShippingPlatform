using BuildingBlocks.Application;
using Identity.Application.Abstractions;
using MediatR;

namespace Identity.Application.Impersonation.ImpersonateCustomer;

public sealed record ImpersonateCustomerCommand(
    Guid ImpersonatorUserId,
    Guid TargetCustomerUserId,
    string? IpAddress,
    string? UserAgent,
    string? Reason)
    : IRequest<Result<TokenPair>>;