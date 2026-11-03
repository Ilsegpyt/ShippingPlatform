using BuildingBlocks.Application;
using MediatR;

namespace Operations.Application.Operations.DeactivateTransporter;

public sealed record DeactivateTransporterCommand(Guid Id) : IRequest<Result<bool>>;
