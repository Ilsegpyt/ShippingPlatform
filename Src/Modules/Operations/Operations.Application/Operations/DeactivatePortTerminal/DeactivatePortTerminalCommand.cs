using BuildingBlocks.Application;
using MediatR;

namespace Operations.Application.Operations.DeactivatePortTerminal;

public sealed record DeactivatePortTerminalCommand(Guid Id) : IRequest<Result<bool>>;
