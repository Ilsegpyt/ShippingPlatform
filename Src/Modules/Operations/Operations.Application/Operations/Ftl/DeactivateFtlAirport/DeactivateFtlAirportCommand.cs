using BuildingBlocks.Application;
using MediatR;

namespace Operations.Application.Operations.Ftl.DeactivateFtlAirport;

public sealed record DeactivateFtlAirportCommand(Guid Id) : IRequest<Result<bool>>;
