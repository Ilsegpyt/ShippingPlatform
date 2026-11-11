using BuildingBlocks.Application;
using MediatR;

namespace Operations.Application.Operations.Ftl.CreateFtlOperation;

public sealed record CreateFtlTripInput(
    string? Id,
    int Ordinal,
    Dictionary<string, string> Fields,
    string? LoadStatus = null,
    int? LoadedPackages = null,
    decimal? LoadedWeight = null,
    decimal? LoadedCbm = null);

public sealed record CreateFtlOperationCommand(
    string Direction,
    int? RowCount,
    int? ContainerCount,
    Dictionary<string, string>? Shared,
    List<CreateFtlTripInput>? Trips,
    string? RequestId) : IRequest<Result<CreateFtlOperationResult>>;

public sealed record CreateFtlOperationResult(
    string Ref,
    int Version,
    string Day,
    string At,
    IReadOnlyList<CreateFtlTripRef> Trips);

public sealed record CreateFtlTripRef(string Id);
