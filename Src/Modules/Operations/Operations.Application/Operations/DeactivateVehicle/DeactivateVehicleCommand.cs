using BuildingBlocks.Application;
using MediatR;

namespace Operations.Application.Operations.DeactivateVehicle;

public sealed record DeactivateVehicleCommand(Guid Id) : IRequest<Result<bool>>;
