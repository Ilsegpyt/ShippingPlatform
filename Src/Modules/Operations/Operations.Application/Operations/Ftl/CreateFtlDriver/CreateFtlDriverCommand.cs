using BuildingBlocks.Application;
using MediatR;

namespace Operations.Application.Operations.Ftl.CreateFtlDriver;

public sealed record CreateFtlDriverCommand(string Name, string? MobileNumber, string? DriverNumber) : IRequest<Result<Guid>>;
