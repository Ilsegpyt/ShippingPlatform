using BuildingBlocks.Application;
using MediatR;

namespace Operations.Application.Operations.UpdateVehicle;

public sealed record UpdateVehicleCommand(Guid Id, string CarNumber, bool IsActive) : IRequest<Result<bool>>;
