using BuildingBlocks.Application;
using MediatR;

namespace Operations.Application.Operations.DeactivatePort;

public sealed record DeactivatePortCommand(Guid Id) : IRequest<Result<bool>>;
