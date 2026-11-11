using BuildingBlocks.Application;
using MediatR;

namespace Operations.Application.Operations.Ftl.DeleteFtlOperation;

public sealed record DeleteFtlOperationCommand(
    string Ref,
    int? Version) : IRequest<Result<DeleteFtlOperationResult>>;

public sealed record DeleteFtlOperationResult(int Version);
