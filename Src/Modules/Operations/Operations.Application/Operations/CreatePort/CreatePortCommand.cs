using BuildingBlocks.Application;
using MediatR;

namespace Operations.Application.Operations.CreatePort;

public sealed record CreatePortCommand(string Name, string? Code) : IRequest<Result<Guid>>;
