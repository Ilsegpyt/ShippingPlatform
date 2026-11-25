using Operations.Domain.Entities.Ftl;

namespace Operations.Application.Abstractions;

public sealed record FtlOperationQueryParams(
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
    string? RulesJson = null);

public sealed record PagedFtlOperationsResult(
    IReadOnlyList<FtlOperation> Items,
    int TotalCount,
    int TotalTrucksCount,
    int Page,
    int PageSize,
    int TotalPages);

public sealed record FtlFilterOptionsDto(
    IReadOnlyList<string> Clients,
    IReadOnlyList<string> Transporters,
    IReadOnlyList<string> Terminals,
    IReadOnlyList<string> Users);
