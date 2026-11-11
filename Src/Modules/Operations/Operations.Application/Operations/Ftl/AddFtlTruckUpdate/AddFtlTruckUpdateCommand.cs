using BuildingBlocks.Application;
using MediatR;

namespace Operations.Application.Operations.Ftl.AddFtlTruckUpdate;

public sealed record AddFtlTruckUpdateCommand(
    string Ref,
    string? TripId,
    string Note) : IRequest<Result<AddFtlTruckUpdateResult>>;

public sealed record AddFtlTruckUpdateResult(
    string Id,
    string Text,
    string At,
    string By);
