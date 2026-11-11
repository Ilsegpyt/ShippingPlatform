using BuildingBlocks.Application;
using MediatR;
using Operations.Application.Operations.Ftl.Common;

namespace Operations.Application.Operations.Ftl.UpdateFtlOperation;

public sealed record UpdateFtlOperationCommand(
    int? Version,
    List<FtlOperationDto> Shipments) : IRequest<Result<UpdateFtlOperationResult>>;

public sealed record UpdateFtlOperationResult(int Version);

