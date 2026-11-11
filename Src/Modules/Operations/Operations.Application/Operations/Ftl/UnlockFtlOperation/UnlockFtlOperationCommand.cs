using BuildingBlocks.Application;
using MediatR;

namespace Operations.Application.Operations.Ftl.UnlockFtlOperation;

public sealed record UnlockFtlOperationCommand(string Ref) : IRequest<Result<bool>>;
