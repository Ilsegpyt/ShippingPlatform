using BuildingBlocks.Application;
using MediatR;
using Operations.Application.Abstractions;
using Operations.Application.Operations.Ftl.Common;

namespace Operations.Application.Operations.Ftl.GetPagedFtlOperations;

public sealed class GetPagedFtlOperationsQueryHandler(
    IFtlOperationRepository repository)
    : IRequestHandler<GetPagedFtlOperationsQuery, Result<PagedFtlOperationsResponseDto>>
{
    public async Task<Result<PagedFtlOperationsResponseDto>> Handle(
        GetPagedFtlOperationsQuery query,
        CancellationToken ct)
    {
        var queryParams = new FtlOperationQueryParams(
            Page: query.Page,
            PageSize: query.PageSize,
            OperationType: query.OperationType,
            Status: query.Status,
            OnlyActive: query.OnlyActive,
            IsDeleted: query.IsDeleted,
            Search: query.Search,
            Client: query.Client,
            Transporter: query.Transporter,
            Terminal: query.Terminal,
            CreatedBy: query.CreatedBy,
            DateType: query.DateType,
            DateFrom: query.DateFrom,
            DateTo: query.DateTo,
            SortBy: query.SortBy,
            SortDirection: query.SortDirection);

        var result = await repository.GetPagedAsync(queryParams, ct);

        var dtos = result.Items
            .Select(FtlOperationMapper.MapToDto)
            .ToList();

        var response = new PagedFtlOperationsResponseDto(
            Items: dtos,
            TotalCount: result.TotalCount,
            TotalTrucksCount: result.TotalTrucksCount,
            Page: result.Page,
            PageSize: result.PageSize,
            TotalPages: result.TotalPages);

        return Result.Success(response);
    }
}
