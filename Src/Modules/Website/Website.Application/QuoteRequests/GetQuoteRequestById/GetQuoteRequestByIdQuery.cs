using BuildingBlocks.Application;
using MediatR;
using Website.Domain.Entities;

namespace Website.Application.QuoteRequests.GetQuoteRequestById;

public sealed record GetQuoteRequestByIdQuery(
    int Id)
    : IRequest<Result<QuoteRequest>>;