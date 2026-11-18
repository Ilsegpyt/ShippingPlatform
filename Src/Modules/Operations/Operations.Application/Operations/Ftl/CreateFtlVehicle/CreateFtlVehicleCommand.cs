using BuildingBlocks.Application;
using MediatR;

namespace Operations.Application.Operations.Ftl.CreateFtlVehicle;

public sealed record CreateFtlVehicleCommand(string PlateNumber, string? TruckType, Guid? FtlTransporterId) : IRequest<Result<Guid>>;
