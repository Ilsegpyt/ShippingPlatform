using BuildingBlocks.Application;
using MediatR;
using Operations.Application.Operations.Ftl.LockFtlOperation;

namespace Operations.Application.Operations.Ftl.TakeOverFtlOperation;

public sealed record TakeOverFtlOperationCommand(string Ref)
    : IRequest<Result<LockFtlOperationResult>>;
