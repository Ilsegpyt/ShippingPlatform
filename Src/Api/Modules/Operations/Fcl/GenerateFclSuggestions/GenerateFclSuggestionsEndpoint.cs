using Identity.Domain.ValueObjects;
using Identity.Infrastructure.Authorization;
using MediatR;
using Operations.Application.Operations.GenerateFclSuggestions;

namespace Api.Modules.Operations.GenerateFclSuggestions;

public static class GenerateFclSuggestionsEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        // Global generation: no Import/Export container ID is required.
        app.MapPost(
            "/api/operations/fcl-suggestions/generate",
            async (ISender sender, CancellationToken ct) =>
            {
                var result = await sender.Send(
                    new GenerateFclSuggestionsCommand(),
                    ct);

                return result.IsSuccess
                    ? Results.Ok(new { CreatedCount = result.Value.Count, Suggestions = result.Value })
                    : Results.BadRequest(new { Error = result.Error });
            })
            .RequirePermission(PermissionCatalog.OperationsEdit);

        // Keep the existing targeted endpoint for backward compatibility.
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
                    ? Results.Ok(new { CreatedCount = result.Value.Count, Suggestions = result.Value })
                    : Results.BadRequest(new { Error = result.Error });
            })
            .RequirePermission(PermissionCatalog.OperationsEdit);
    }
}
