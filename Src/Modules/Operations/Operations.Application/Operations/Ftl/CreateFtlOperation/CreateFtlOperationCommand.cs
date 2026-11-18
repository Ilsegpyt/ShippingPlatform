using System.Text.Json.Serialization;
using BuildingBlocks.Application;
using BuildingBlocks.Application.Serialization;
using MediatR;

namespace Operations.Application.Operations.Ftl.CreateFtlOperation;

public sealed record CreateFtlTripInput(
    string? Id,
    int Ordinal,
    [property: JsonConverter(typeof(SafeStringDictionaryConverter))]
    Dictionary<string, string> Fields,
    string? LoadStatus = null,
    int? LoadedPackages = null,
    decimal? LoadedWeight = null,
    decimal? LoadedCbm = null);

public sealed record CreateFtlOperationCommand(
    string Direction,
    int? RowCount,
    int? ContainerCount,
    int? Shipments,
    int? Trucks,
    [property: JsonConverter(typeof(SafeStringDictionaryConverter))]
    Dictionary<string, string>? Shared,
    List<CreateFtlTripInput>? Trips,
    string? RequestId) : IRequest<Result<CreateFtlOperationResult>>;

public sealed record CreateFtlOperationResult(
    string Ref,
    string Day,
    string At,
    IReadOnlyList<CreateFtlTripRef> Trips,
    int? Version = null);

public sealed record CreateFtlTripRef(string Id);
