using Identity.Domain.ValueObjects;
using Identity.Infrastructure.Authorization;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Operations.Application.Operations.Ftl.AddFtlMaster;

namespace Api.Modules.Operations.AddFtlMasterItem;

public static class AddFtlMasterItemEndpoint
{
    public sealed record AddMasterItemRequest(string? Value);

    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/operations/ftl/master/{list}", HandleAddItem)
            .RequirePermission(PermissionCatalog.OperationsCreate);

        // Support front routes
        app.MapPost("/api/v1/ftl/master/{list}", HandleAddItem)
            .RequirePermission(PermissionCatalog.OperationsCreate);
        app.MapPost("/master/{list}", HandleAddItem)
            .RequirePermission(PermissionCatalog.OperationsCreate);
    }

    private static async Task<IResult> HandleAddItem(
        string list,
        [FromBody] AddMasterItemRequest request,
        ISender sender,
        CancellationToken ct)
    {
        var val = request.Value ?? string.Empty;
        var command = new AddFtlMasterCommand(list, val);
        var result = await sender.Send(command, ct);

        return result.IsSuccess
            ? Results.Ok(new { value = result.Value })
            : Results.BadRequest(new { error = result.Error });
    }
}

