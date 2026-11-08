using System;
using BuildingBlocks.Application;
using MediatR;
using Operations.Application.Operations.LockOperation;

namespace Operations.Application.Operations.TakeOverOperation;

public sealed record TakeOverOperationCommand(Guid OperationId)
    : IRequest<Result<OperationLockDto>>;

