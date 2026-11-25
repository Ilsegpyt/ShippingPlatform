using BuildingBlocks.Application;
using MediatR;
using Operations.Application.Operations.Ftl.Common;

namespace Operations.Application.Operations.Ftl.GetPagedFtlOperations;

public sealed record PagedFtlOperationsResponseDto(
    IReadOnlyList<FtlOperationDto> Items,
    int TotalCount,
    int TotalTrucksCount,
    int Page,
    int PageSize,
    int TotalPages);

public sealed record GetPagedFtlOperationsQuery(
    int Page = 1,
    int PageSize = 20,
    string? OperationType = null,
    string? Status = null,
    bool OnlyActive = false,
    bool IsDeleted = false,
    string? Search = null,
    string? Client = null,
    string? Transporter = null,
    string? Terminal = null,
    string? CreatedBy = null,
    string? DateType = null,
    string? DateFrom = null,
    string? DateTo = null,
    string? SortBy = null,
    string? SortDirection = "desc",
    string? RulesJson = null)
    : IRequest<Result<PagedFtlOperationsResponseDto>>;
