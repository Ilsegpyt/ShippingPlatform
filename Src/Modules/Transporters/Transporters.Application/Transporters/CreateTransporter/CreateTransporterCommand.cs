using BuildingBlocks.Application;
using MediatR;

namespace Transporters.Application.Transporters.CreateTransporter;

public sealed record CreateTransporterCommand(
    string Name,
    string? ContactPerson,
    string? Phone,
    string? Email)
    : IRequest<Result<Guid>>;
