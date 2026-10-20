using Operations.Domain.Entities;

namespace Operations.Application.Operations.SearchOperations;

public sealed record OperationSearchPage(
    IReadOnlyList<Operation> Items,
    int TotalCount);
