using BuildingBlocks.Application;
using Identity.Application.Abstractions;
using MediatR;

namespace Identity.Application.Impersonation.ImpersonateCustomer;

public sealed record ImpersonateCustomerCommand(
    Guid CustomerUserId) : IRequest<Result<TokenPair>>;