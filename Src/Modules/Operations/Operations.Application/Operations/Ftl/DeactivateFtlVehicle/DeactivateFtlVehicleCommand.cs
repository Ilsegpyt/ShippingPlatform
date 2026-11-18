using BuildingBlocks.Application;
using MediatR;

namespace Operations.Application.Operations.Ftl.DeactivateFtlVehicle;

public sealed record DeactivateFtlVehicleCommand(Guid Id) : IRequest<Result<bool>>;
