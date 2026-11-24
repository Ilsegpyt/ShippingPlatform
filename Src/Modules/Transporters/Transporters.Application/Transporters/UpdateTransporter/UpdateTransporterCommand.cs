using BuildingBlocks.Application;
using MediatR;

namespace Transporters.Application.Transporters.UpdateTransporter;

public sealed record UpdateTransporterCommand(
    Guid Id,
    string Name,
    string? ContactPerson,
    string? Phone,
    string? Email,
    bool IsActive)
    : IRequest<Result>;
