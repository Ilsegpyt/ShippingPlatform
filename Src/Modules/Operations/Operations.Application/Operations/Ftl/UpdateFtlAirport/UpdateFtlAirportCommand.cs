using BuildingBlocks.Application;
using MediatR;

namespace Operations.Application.Operations.Ftl.UpdateFtlAirport;

public sealed record UpdateFtlAirportCommand(Guid Id, string Name, string? Code, bool IsActive) : IRequest<Result<bool>>;
