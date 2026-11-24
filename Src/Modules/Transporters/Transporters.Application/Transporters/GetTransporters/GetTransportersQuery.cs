using BuildingBlocks.Application;
using MediatR;
using Transporters.Domain.Entities;

namespace Transporters.Application.Transporters.GetTransporters;

public sealed record GetTransportersQuery(
    PaginationRequest Pagination)
    : IRequest<Result<PagedResult<Transporter>>>;
