using BuildingBlocks.Application;
using MediatR;

namespace Operations.Application.Operations.CreateTransporter;

public sealed record CreateTransporterCommand(string Name) : IRequest<Result<Guid>>;
