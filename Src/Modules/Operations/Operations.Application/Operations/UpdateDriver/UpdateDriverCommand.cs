using BuildingBlocks.Application;
using MediatR;

namespace Operations.Application.Operations.UpdateDriver;

public sealed record UpdateDriverCommand(Guid Id, string DriverNumber, string Name, string? MobileNumber, bool IsActive) : IRequest<Result<bool>>;
