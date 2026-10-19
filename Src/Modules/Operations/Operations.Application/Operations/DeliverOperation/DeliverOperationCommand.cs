using BuildingBlocks.Application;
using MediatR;

namespace Operations.Application.Operations.DeliverOperation;

public sealed record DeliverOperationCommand(Guid OperationId)
    : IRequest<Result<bool>>;
