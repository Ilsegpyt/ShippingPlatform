using BuildingBlocks.Application;
using MediatR;

namespace Operations.Application.Operations.Ftl.DeactivateFtlTerminal;

public sealed record DeactivateFtlTerminalCommand(Guid Id) : IRequest<Result<bool>>;
