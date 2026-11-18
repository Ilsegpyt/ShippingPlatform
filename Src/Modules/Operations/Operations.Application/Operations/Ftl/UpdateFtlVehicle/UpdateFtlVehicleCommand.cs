using BuildingBlocks.Application;
using MediatR;

namespace Operations.Application.Operations.Ftl.UpdateFtlVehicle;

public sealed record UpdateFtlVehicleCommand(Guid Id, string PlateNumber, string? TruckType, Guid? FtlTransporterId, bool IsActive) : IRequest<Result<bool>>;
