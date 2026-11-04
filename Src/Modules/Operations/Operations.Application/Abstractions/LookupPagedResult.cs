namespace Operations.Application.Abstractions;

public sealed record LookupPagedResult<T>(
    IReadOnlyList<T> Items,
    int TotalCount,
    int ActiveCount,
    int InactiveCount);

