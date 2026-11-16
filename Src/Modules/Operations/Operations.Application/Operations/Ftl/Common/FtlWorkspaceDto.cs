using System.Text.Json.Serialization;
using BuildingBlocks.Application.Serialization;

namespace Operations.Application.Operations.Ftl.Common;

public sealed record FtlWorkspaceDto(
    int Version,
    IReadOnlyList<FtlOperationDto> Shipments);

public sealed record FtlOperationDto(
    string Ref,
    string Direction,
    string Status,
    string Day,
    string At,
    FtlUserDto EnteredBy,
    IReadOnlyList<string> Unmerged,
    IReadOnlyList<FtlTripDto> Trips,
    int? Per = 1,
    int? Shp = 1,
    int? Trk = 1,
    IReadOnlyList<IReadOnlyList<int>>? ShpSpans = null,
    IReadOnlyList<IReadOnlyList<int>>? TrkSpans = null,
    IReadOnlyList<int>? TrkOf = null);

public sealed record FtlUserDto(
    string Id,
    string DisplayName);

public sealed record FtlTripDto(
    string Id,
    int Ordinal,
    [property: JsonConverter(typeof(SafeStringDictionaryConverter))]
    Dictionary<string, string> Fields,
    IReadOnlyList<FtlUpdateDto> Updates,
    string? LoadStatus = "Full",
    int? LoadedPackages = null,
    decimal? LoadedWeight = null,
    decimal? LoadedCbm = null);

public sealed record FtlUpdateDto(
    string? Id = null,
    string? Text = null,
    string? At = null,
    string? By = null,
    string? Day = null,
    string? User = null,
    string? Note = null);
