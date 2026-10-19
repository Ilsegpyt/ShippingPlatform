
using Identity.Domain.ValueObjects;
using Identity.Infrastructure.Authorization;
using MediatR;
using Operations.Application.Operations.GenerateFclSuggestions;

namespace Api.Modules.Operations.GenerateFclSuggestions;

public static class GenerateFclSuggestionsEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPost(
            "/api/operations/exports/containers/{exportContainerId:guid}/fcl-suggestions/generate",
            async (
                Guid exportContainerId,
                ISender sender,
                CancellationToken ct) =>
            {
                var result = await sender.Send(
                    new GenerateFclSuggestionsCommand(exportContainerId),
                    ct);

                return result.IsSuccess
                    ? Results.Ok(new { CreatedCount = result.Value })
                    : Results.BadRequest(new { Error = result.Error });
            })
            .RequirePermission(PermissionCatalog.OperationsEdit);
    }
}
