using BuildingBlocks.Application;
using MediatR;
using Website.Application.ContactInquiries.GetContactInquiries;

namespace Api.Modules.Website.ContactInquiries.GetContactInquiries;

public static class GetContactInquiriesEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapGet(
            "/api/website/contact-inquiries",
            async (
                [AsParameters] PaginationRequest pagination,
                ISender sender,
                CancellationToken ct) =>
            {
                var query = new GetContactInquiriesQuery(
                    pagination);

                var result = await sender.Send(query, ct);

                return result.IsSuccess
                    ? Results.Ok(result.Value)
                    : Results.BadRequest(result.Error);
            });
    }
}