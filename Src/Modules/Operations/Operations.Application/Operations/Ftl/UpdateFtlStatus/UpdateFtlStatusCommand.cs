using BuildingBlocks.Application;
using MediatR;

namespace Operations.Application.Operations.Ftl.UpdateFtlStatus;

public sealed record UpdateFtlStatusCommand(
    string Ref,
    string Status,
    int? Version) : IRequest<Result<UpdateFtlStatusResult>>;

public sealed record UpdateFtlStatusResult(int Version);

