using BuildingBlocks.Application;
using MediatR;

namespace Operations.Application.Operations.CreateDriver;

public sealed record CreateDriverCommand(string DriverNumber, string Name, string? MobileNumber) : IRequest<Result<Guid>>;
