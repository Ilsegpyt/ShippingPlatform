using System;
using BuildingBlocks.Application;
using MediatR;

namespace Operations.Application.Operations.UnlockOperation;

public sealed record UnlockOperationCommand(Guid OperationId, bool Force = false)
    : IRequest<Result<bool>>;

