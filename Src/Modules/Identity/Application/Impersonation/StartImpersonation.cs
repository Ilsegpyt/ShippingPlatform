using BuildingBlocks.Application;
using Identity.Application.Abstractions;
using MediatR;

// to be revised
public sealed record StartImpersonationCommand(
    Guid TargetCustomerUserId) : IRequest<Result<TokenPair>>;