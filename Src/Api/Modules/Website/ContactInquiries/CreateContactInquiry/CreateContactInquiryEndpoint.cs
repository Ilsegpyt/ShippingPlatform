using MediatR;
using Website.Application.ContactInquiries.CreateContactInquiry;

namespace Api.Modules.Website.ContactInquiries.CreateContactInquiry;

public static class CreateContactInquiryEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPost(
            "/api/website/contact-inquiries",
            async (
                CreateContactInquiryRequest request,
                ISender sender,
                CancellationToken ct) =>
            {
                var command = new CreateContactInquiryCommand(
                    request.FullName,
                    request.Company,
                    request.Email,
                    request.Phone,
                    request.Service,
                    request.Message);

                var result = await sender.Send(command, ct);

                return result.IsSuccess
                    ? Results.Ok()
                    : Results.BadRequest(result.Error);
            });
    }
}

public sealed record CreateContactInquiryRequest(
    string FullName,
    string Company,
    string Email,
    string Phone,
    string Service,
    string Message);