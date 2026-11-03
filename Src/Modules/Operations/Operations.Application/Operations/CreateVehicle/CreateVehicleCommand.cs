using BuildingBlocks.Application;
using MediatR;

namespace Operations.Application.Operations.CreateVehicle;

public sealed record CreateVehicleCommand(string CarNumber) : IRequest<Result<Guid>>;
