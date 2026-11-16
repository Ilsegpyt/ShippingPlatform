using Identity.Domain.ValueObjects;
using Identity.Infrastructure.Authorization;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Operations.Application.Operations.Ftl.CreateFtlOperation;

namespace Api.Modules.Operations.CreateFtlOperation;

public static class CreateFtlOperationEndpoint
{
    public sealed record CreateFtlTripRequest(
        string? Id,
        int Ordinal,
        Dictionary<string, string> Fields,
        string? LoadStatus,
        int? LoadedPackages,
        decimal? LoadedWeight,
        decimal? LoadedCbm);

    public sealed record CreateFtlOperationRequest(
        string Direction,
        int? RowCount,
        int? ContainerCount,
        Dictionary<string, string>? Shared,
        List<CreateFtlTripRequest>? Trips,
        string? RequestId);

    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/operations/ftl", HandleCreate)
            .RequirePermission(PermissionCatalog.OperationsCreate);

        // Also support legacy/front routes
        app.MapPost("/api/v1/ftl/operations", HandleCreate)
            .RequirePermission(PermissionCatalog.OperationsCreate);
        app.MapPost("/createOperation", HandleCreate)
            .RequirePermission(PermissionCatalog.OperationsCreate);
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
            request.Shared,
            trips,
            request.RequestId);

        var result = await sender.Send(command, ct);

        return result.IsSuccess
            ? Results.Ok(result.Value)
            : Results.BadRequest(new { error = result.Error });
    }
}

