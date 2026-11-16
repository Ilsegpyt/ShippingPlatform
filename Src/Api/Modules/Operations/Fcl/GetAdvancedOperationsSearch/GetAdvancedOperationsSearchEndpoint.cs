using Identity.Domain.ValueObjects;
using Identity.Infrastructure.Authorization;
using MediatR;
using Operations.Application.Operations.SearchOperations;

namespace Api.Modules.Operations.GetAdvancedOperationsSearch;

public static class GetAdvancedOperationsSearchEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/operations/search", async (
            AdvancedOperationsSearchRequest request,
            ISender sender,
            CancellationToken ct) =>
        {
            if (request.Page < 1 || request.PageSize < 1 || request.PageSize > 200)
                return Results.BadRequest(new { Error = "Page must be >= 1 and PageSize must be between 1 and 200." });

            if (request.Rules is { Count: > 20 })
                return Results.BadRequest(new { Error = "A maximum of 20 search rules is allowed." });

            var result = await sender.Send(new SearchOperationsQuery(request), ct);

            return result.IsSuccess
                ? Results.Ok(result.Value)
                : Results.BadRequest(new { Error = result.Error });
        })
        .RequirePermission(PermissionCatalog.OperationsView);
    }
}
