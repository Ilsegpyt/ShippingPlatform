
using Identity.Domain.ValueObjects;
using Identity.Infrastructure.Authorization;
using MediatR;
using Operations.Application.Operations.ApproveFclSuggestion;

namespace Api.Modules.Operations.ApproveFclSuggestion;

public static class ApproveFclSuggestionEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPost(
            "/api/fcl-suggestions/{suggestionId:guid}/approve",
            async (
                Guid suggestionId,
                ISender sender,
                CancellationToken ct) =>
            {
                var result = await sender.Send(
                    new ApproveFclSuggestionCommand(
                        suggestionId,
                        false),
                    ct);

                return result.IsSuccess
                    ? Results.Ok(new { ApprovalId = result.Value })
                    : Results.BadRequest(new { Error = result.Error });
            })
            .RequirePermission(PermissionCatalog.OperationsEdit);
    }
}
