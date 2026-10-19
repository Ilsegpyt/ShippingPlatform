using BuildingBlocks.Application;
using MediatR;

namespace Operations.Application.Operations.ReopenOperation;

public sealed record ReopenOperationCommand(
    Guid OperationId,
    string Reason) : IRequest<Result<bool>>;
