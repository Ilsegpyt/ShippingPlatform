using BuildingBlocks.Application;
using MediatR;

namespace Operations.Application.Operations.UpdateTransporter;

public sealed record UpdateTransporterCommand(Guid Id, string Name, bool IsActive) : IRequest<Result<bool>>;
