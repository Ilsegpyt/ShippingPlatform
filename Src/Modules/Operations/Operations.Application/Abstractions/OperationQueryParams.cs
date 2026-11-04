using Operations.Domain.Entities;

namespace Operations.Application.Abstractions;

public sealed record OperationQueryParams(
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
    string? DateTo = null,
    IReadOnlyCollection<Guid>? MatchingClientIds = null,
    IReadOnlyCollection<Guid>? MatchingFilterClientIds = null);

public sealed record PagedOperationsResult(
    IReadOnlyList<Operation> Items,
    int TotalCount,
    int TotalContainersCount,
    int Page,
    int PageSize,
    int TotalPages);
