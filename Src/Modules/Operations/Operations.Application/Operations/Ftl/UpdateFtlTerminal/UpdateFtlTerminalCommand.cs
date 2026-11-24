using BuildingBlocks.Application;
using MediatR;

namespace Operations.Application.Operations.Ftl.UpdateFtlTerminal;

public sealed record UpdateFtlTerminalCommand(Guid Id, string Name, bool IsActive) : IRequest<Result<bool>>;
