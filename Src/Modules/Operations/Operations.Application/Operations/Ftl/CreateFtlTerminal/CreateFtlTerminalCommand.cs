using BuildingBlocks.Application;
using MediatR;

namespace Operations.Application.Operations.Ftl.CreateFtlTerminal;

public sealed record CreateFtlTerminalCommand(string Name) : IRequest<Result<Guid>>;
