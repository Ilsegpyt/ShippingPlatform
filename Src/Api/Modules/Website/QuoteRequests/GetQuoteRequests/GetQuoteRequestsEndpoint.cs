using BuildingBlocks.Application;
using MediatR;
using Website.Application.QuoteRequests.GetQuoteRequests;

namespace Api.Modules.Website.QuoteRequests.GetQuoteRequests;

public static class GetQuoteRequestsEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapGet(
            "/api/website/quote-requests",
            async (
                [AsParameters] PaginationRequest pagination,
                ISender sender,
                CancellationToken ct) =>
            {
                var query = new GetQuoteRequestsQuery(
                    pagination);

                var result = await sender.Send(query, ct);

                return result.IsSuccess
                    ? Results.Ok(result.Value)
                    : Results.BadRequest(result.Error);
            });
    }
}