using System;
using BuildingBlocks.Application;
using MediatR;

namespace Operations.Application.Operations.LockOperation;

public sealed record LockOperationCommand(Guid OperationId)
    : IRequest<Result<OperationLockDto>>;

