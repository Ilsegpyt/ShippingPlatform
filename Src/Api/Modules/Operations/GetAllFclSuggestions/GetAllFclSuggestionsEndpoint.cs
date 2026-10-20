using Identity.Domain.ValueObjects;
using Identity.Infrastructure.Authorization;
using MediatR;
using Operations.Application.Operations.GetAllFclSuggestions;

namespace Api.Modules.Operations.GetAllFclSuggestions;

public static class GetAllFclSuggestionsEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapGet(
            "/api/operations/fcl-suggestions",
            async (ISender sender, CancellationToken ct) =>
            {
                var result = await sender.Send(
                    new GetAllFclSuggestionsQuery(), ct);

                return result.IsSuccess
                    ? Results.Ok(result.Value)
                    : Results.BadRequest(new { Error = result.Error });
            })
            .RequirePermission(PermissionCatalog.OperationsView);
    }
}