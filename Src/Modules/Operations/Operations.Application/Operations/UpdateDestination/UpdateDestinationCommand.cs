using BuildingBlocks.Application;
using MediatR;

namespace Operations.Application.Operations.UpdateDestination;

public sealed record UpdateDestinationCommand(Guid Id, string Name, bool IsActive) : IRequest<Result<bool>>;
