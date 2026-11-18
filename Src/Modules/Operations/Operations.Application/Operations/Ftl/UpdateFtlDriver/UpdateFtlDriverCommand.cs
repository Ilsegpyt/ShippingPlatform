using BuildingBlocks.Application;
using MediatR;

namespace Operations.Application.Operations.Ftl.UpdateFtlDriver;

public sealed record UpdateFtlDriverCommand(Guid Id, string Name, string? MobileNumber, string? DriverNumber, bool IsActive) : IRequest<Result<bool>>;
