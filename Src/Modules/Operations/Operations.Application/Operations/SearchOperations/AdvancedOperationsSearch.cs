namespace Operations.Application.Operations.SearchOperations;

public sealed record AdvancedOperationsSearchRequest(
    IReadOnlyList<OperationSearchRule>? Rules,
    string? SortBy,
    string? SortDirection,
    string? OperationType,
    string? Status,
    int Page = 1,
    int PageSize = 50);

public sealed record OperationSearchRule(
    string Field,
    string Operator,
    string? Value,
    string? Logic = "AND");

public sealed record AdvancedOperationsSearchResponse<T>(
    IReadOnlyList<T> Items,
    int TotalCount,
    int Page,
    int PageSize);
