using BuildingBlocks.Application;
using MediatR;
using Website.Application.Abstractions;
using Website.Application.Abstractions.Repositories;
using Website.Domain.Entities;

namespace Website.Application.QuoteRequests.CreateQuoteRequest;

public sealed class CreateQuoteRequestCommandHandler(
    IQuoteRequestRepository quoteRequestRepository,
    IWebsiteUnitOfWork websiteUnitOfWork)
    : IRequestHandler<CreateQuoteRequestCommand, Result>
{
    public async Task<Result> Handle(
        CreateQuoteRequestCommand cmd,
        CancellationToken ct)
    {
        var request = new QuoteRequest
        {
            FirstName = cmd.FirstName,
            LastName = cmd.LastName,
            Company = cmd.Company,
            Country = cmd.Country,
            CountryCode = cmd.CountryCode,
            Phone = cmd.Phone,
            Email = cmd.Email,
            Message = cmd.Message,
            InterestType = cmd.InterestType,

            TransportModes = cmd.TransportModes.ToList(),

            AnnualShipments = cmd.AnnualShipments,
            CreatedAtUtc = DateTime.UtcNow
        };

        quoteRequestRepository.Add(request);

        await websiteUnitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }
}