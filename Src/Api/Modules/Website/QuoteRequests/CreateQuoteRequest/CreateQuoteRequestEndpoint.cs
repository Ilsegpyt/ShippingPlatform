using MediatR;
using Website.Application.QuoteRequests.CreateQuoteRequest;
using Website.Domain.Enums;

namespace Api.Modules.Website.QuoteRequests.CreateQuoteRequest;

public static class CreateQuoteRequestEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPost(
            "/api/website/quote-requests",
            async (
                CreateQuoteRequestRequest request,
                ISender sender,
                CancellationToken ct) =>
            {
                var command = new CreateQuoteRequestCommand(
                    request.FirstName,
                    request.LastName,
                    request.Company,
                    request.Country,
                    request.CountryCode,
                    request.Phone,
                    request.Email,
                    request.Message,
                    request.InterestType,
                    request.TransportModes,
                    request.AnnualShipments);

                var result = await sender.Send(command, ct);

                return result.IsSuccess
                    ? Results.Ok()
                    : Results.BadRequest(result.Error);
            });
    }
}

public sealed record CreateQuoteRequestRequest(
    string FirstName,
    string LastName,
    string Company,
    string Country,
    string CountryCode,
    string Phone,
    string Email,
    string Message,
    InterestType InterestType,
    IReadOnlyList<TransportMode> TransportModes,
    AnnualShipments AnnualShipments);