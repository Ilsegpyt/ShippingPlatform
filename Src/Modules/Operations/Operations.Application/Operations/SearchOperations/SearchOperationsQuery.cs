using BuildingBlocks.Application;
using MediatR;
using Operations.Application.Operations.GetAllOperations;

namespace Operations.Application.Operations.SearchOperations;

public sealed record SearchOperationsQuery(AdvancedOperationsSearchRequest Request)
    : IRequest<Result<AdvancedOperationsSearchResponse<OperationDto>>>;
