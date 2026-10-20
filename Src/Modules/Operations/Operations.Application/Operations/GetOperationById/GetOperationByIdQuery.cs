using BuildingBlocks.Application;
using MediatR;
using Operations.Application.Operations.GetAllOperations;

namespace Operations.Application.Operations.GetOperationById;

public sealed record GetOperationByIdQuery(Guid Id)
    : IRequest<Result<OperationDto>>;
