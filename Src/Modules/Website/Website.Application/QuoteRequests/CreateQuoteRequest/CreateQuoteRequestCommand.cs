using BuildingBlocks.Application;
using MediatR;
using Website.Domain.Enums;

namespace Website.Application.QuoteRequests.CreateQuoteRequest;

public sealed record CreateQuoteRequestCommand(
    string FirstName,
    string LastName,
    string Company,
    string Country,
    string CountryCode,
    string Phone,
    string Email,
    string Message,
    InterestType InterestType,
    TransportMode TransportMode,
    AnnualShipments AnnualShipments)
    : IRequest<Result>;