using MediatR;
using Website.Application.ContactInquiries.GetContactInquiryById;

namespace Api.Modules.Website.ContactInquiries.GetContactInquiryById;

public static class GetContactInquiryByIdEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapGet(
            "/api/website/contact-inquiries/{id:int}",
            async (
                int id,
                ISender sender,
                CancellationToken ct) =>
            {
                var query = new GetContactInquiryByIdQuery(id);

                var result = await sender.Send(query, ct);

                return result.IsSuccess
                    ? Results.Ok(result.Value)
                    : Results.NotFound(result.Error);
            });
    }
}