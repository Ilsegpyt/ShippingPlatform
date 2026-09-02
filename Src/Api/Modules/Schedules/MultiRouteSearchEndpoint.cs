using MediatR;
using Microsoft.AspNetCore.Mvc;
using Schedules.Application.Schedules.MultiRouteSearch;

namespace Api.Modules.Schedules;

public static class MultiRouteSearchEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/schedules/multi-search", async (
            [FromBody] MultiRouteSearchRequest request,
            ISender sender,
            CancellationToken ct) =>
        {
            var query = new MultiRouteSearchQuery(
                request.Routes);

            var result = await sender.Send(query, ct);

            return result.IsSuccess
                ? Results.Ok(result.Value)
                : Results.BadRequest(result.Error);
        });
    }
}

public sealed record MultiRouteSearchRequest(
    IReadOnlyList<MultiRouteSearchItem> Routes);