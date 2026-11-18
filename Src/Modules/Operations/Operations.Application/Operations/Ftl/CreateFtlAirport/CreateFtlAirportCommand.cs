using BuildingBlocks.Application;
using MediatR;

namespace Operations.Application.Operations.Ftl.CreateFtlAirport;

public sealed record CreateFtlAirportCommand(string Name, string? Code) : IRequest<Result<Guid>>;
