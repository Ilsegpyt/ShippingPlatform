using BuildingBlocks.Application;
using MediatR;
using Operations.Application.Operations.GetAllOperations;
using Operations.Domain.Enums;

namespace Operations.Application.Operations.GetDeliveredOperations;

public sealed record GetDeliveredOperationsQuery(OperationType? OperationType)
    : IRequest<Result<IReadOnlyList<OperationDto>>>;
