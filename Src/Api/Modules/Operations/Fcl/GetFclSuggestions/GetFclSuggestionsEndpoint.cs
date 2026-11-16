
using Identity.Domain.ValueObjects;
using Identity.Infrastructure.Authorization;
using MediatR;
using Operations.Application.Operations.GetFclSuggestions;

namespace Api.Modules.Operations.GetFclSuggestions;

public static class GetFclSuggestionsEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapGet(
            "/api/operations/exports/containers/{exportContainerId:guid}/fcl-suggestions",
            async (
                Guid exportContainerId,
                ISender sender,
                CancellationToken ct) =>
            {
                var result = await sender.Send(
                    new GetFclSuggestionsQuery(exportContainerId),
                    ct);

                return result.IsSuccess
                    ? Results.Ok(result.Value)
                    : Results.BadRequest(new { Error = result.Error });
            })
            .RequirePermission(PermissionCatalog.OperationsView);
    }
}
