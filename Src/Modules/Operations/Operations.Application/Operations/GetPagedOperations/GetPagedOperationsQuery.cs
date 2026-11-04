using BuildingBlocks.Application;
using MediatR;
using Operations.Application.Operations.GetAllOperations;

namespace Operations.Application.Operations.GetPagedOperations;

public sealed record PagedOperationsResponseDto(
    IReadOnlyList<OperationDto> Items,
    int TotalCount,
    int TotalContainersCount,
    int Page,
    int PageSize,
    int TotalPages);

public sealed record GetPagedOperationsQuery(
    int Page = 1,
    int PageSize = 10,
    string? OperationType = null,
    string? Status = null,
    bool OnlyActive = false,
    bool IsDeleted = false,
    string? Search = null,
    string? Client = null,
    string? Line = null,
    string? Port = null,
    string? Transporter = null,
    string? ContainerType = null,
    string? Destination = null,
    string? ClearanceType = null,
    string? MissingDocs = null,
    string? CreatedBy = null,
    string? Customs = null,
    string? DateType = null,
    string? DateFrom = null,
    string? DateTo = null)
    : IRequest<Result<PagedOperationsResponseDto>>;
