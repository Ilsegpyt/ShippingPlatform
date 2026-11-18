using System.Text.Json.Serialization;
using BuildingBlocks.Application.Serialization;
using Identity.Domain.ValueObjects;
using Identity.Infrastructure.Authorization;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Operations.Application.Operations.Ftl.CreateFtlOperation;

namespace Api.Modules.Operations.Ftl.CreateFtlOperation;

public static class CreateFtlOperationEndpoint
{
    public sealed record CreateFtlTripRequest(
        string? Id,
        int Ordinal,
        [property: JsonConverter(typeof(SafeStringDictionaryConverter))]
        Dictionary<string, string> Fields,
        string? LoadStatus,
        int? LoadedPackages,
        decimal? LoadedWeight,
        decimal? LoadedCbm);

    public sealed record CreateFtlOperationRequest(
        string Direction,
        int? RowCount,
        int? ContainerCount,
        int? Shipments,
        int? Trucks,
        [property: JsonConverter(typeof(SafeStringDictionaryConverter))]
        Dictionary<string, string>? Shared,
        List<CreateFtlTripRequest>? Trips,
        string? RequestId);

    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/operations/ftl", HandleCreate)
            .RequirePermission(PermissionCatalog.FtlOperationsCreate);

        // Also support legacy/front routes
        app.MapPost("/api/v1/ftl/operations", HandleCreate)
            .RequirePermission(PermissionCatalog.FtlOperationsCreate);
        app.MapPost("/createOperation", HandleCreate)
            .RequirePermission(PermissionCatalog.FtlOperationsCreate);
    }

    private static async Task<IResult> HandleCreate(
        [FromBody] CreateFtlOperationRequest request,
        ISender sender,
        CancellationToken ct)
    {
        var trips = request.Trips?.Select(t => new CreateFtlTripInput(
            t.Id,
            t.Ordinal,
            t.Fields,
            t.LoadStatus,
            t.LoadedPackages,
            t.LoadedWeight,
            t.LoadedCbm)).ToList();

        var command = new CreateFtlOperationCommand(
            request.Direction,
            request.RowCount,
            request.ContainerCount,
            request.Shipments,
            request.Trucks,
            request.Shared,
            trips,
            request.RequestId);

        var result = await sender.Send(command, ct);

        return result.IsSuccess
            ? Results.Ok(result.Value)
            : Results.BadRequest(new { error = result.Error });
    }
}

