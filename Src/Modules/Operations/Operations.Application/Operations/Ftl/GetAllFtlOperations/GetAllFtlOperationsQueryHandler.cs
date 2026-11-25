using BuildingBlocks.Application;
using MediatR;
using Operations.Application.Abstractions;
using Operations.Application.Operations.Ftl.Common;

namespace Operations.Application.Operations.Ftl.GetAllFtlOperations;

public sealed class GetAllFtlOperationsQueryHandler(
    IFtlOperationRepository operationRepository)
    : IRequestHandler<GetAllFtlOperationsQuery, Result<FtlWorkspaceDto>>
{
    public async Task<Result<FtlWorkspaceDto>> Handle(GetAllFtlOperationsQuery query, CancellationToken ct)
    {
        var operations = await operationRepository.GetWorkspaceWithDetailsAsync(
            query.OperationType,
            query.OnlyActive,
            ct);

        var shipmentDtos = operations
            .Select(FtlOperationMapper.MapToDto)
            .ToList();

        return Result.Success(new FtlWorkspaceDto(shipmentDtos));
    }
}
