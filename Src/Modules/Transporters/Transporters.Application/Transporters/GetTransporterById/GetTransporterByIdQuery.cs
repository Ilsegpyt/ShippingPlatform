using BuildingBlocks.Application;
using MediatR;
using Transporters.Domain.Entities;

namespace Transporters.Application.Transporters.GetTransporterById;

public sealed record GetTransporterByIdQuery(
    Guid Id)
    : IRequest<Result<Transporter>>;
