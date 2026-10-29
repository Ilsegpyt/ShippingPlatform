using BuildingBlocks.Application;
using MediatR;

namespace Operations.Application.Operations.DeleteOperation;

public sealed record DeleteOperationCommand(
    Guid OperationId) : IRequest<Result<bool>>;
