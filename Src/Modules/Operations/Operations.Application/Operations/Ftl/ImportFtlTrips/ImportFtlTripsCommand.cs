using System.Text.Json.Serialization;
using BuildingBlocks.Application;
using BuildingBlocks.Application.Serialization;
using MediatR;

namespace Operations.Application.Operations.Ftl.ImportFtlTrips;

public sealed record ImportFtlTripRow(
    string Direction,
    string? Group,
    [property: JsonConverter(typeof(SafeStringDictionaryConverter))]
    Dictionary<string, string> Fields);

public sealed record ImportFtlTripsCommand(
    string? RequestId,
    List<ImportFtlTripRow> Trips) : IRequest<Result<ImportFtlTripsResult>>;

public sealed record ImportFtlTripsResult(int? Version = null);
