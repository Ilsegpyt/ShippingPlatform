using MediatR;
using Website.Application.QuoteRequests.GetQuoteRequestById;

namespace Api.Modules.Website.QuoteRequests.GetQuoteRequestById;

public static class GetQuoteRequestByIdEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapGet(
            "/api/website/quote-requests/{id:int}",
            async (
                int id,
                ISender sender,
                CancellationToken ct) =>
            {
                var query = new GetQuoteRequestByIdQuery(id);

                var result = await sender.Send(query, ct);

                return result.IsSuccess
                    ? Results.Ok(result.Value)
                    : Results.NotFound(result.Error);
            });
    }
}