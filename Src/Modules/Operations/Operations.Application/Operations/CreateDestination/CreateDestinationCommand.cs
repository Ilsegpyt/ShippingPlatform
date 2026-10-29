using BuildingBlocks.Application;
using MediatR;

namespace Operations.Application.Operations.CreateDestination;

public sealed record CreateDestinationCommand(
    string Name
) : IRequest<Result<Guid>>;
