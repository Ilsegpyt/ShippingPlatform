using BuildingBlocks.Application;
using MediatR;

namespace Operations.Application.Operations.DeactivateDestination;

public sealed record DeactivateDestinationCommand(Guid Id) : IRequest<Result<bool>>;
