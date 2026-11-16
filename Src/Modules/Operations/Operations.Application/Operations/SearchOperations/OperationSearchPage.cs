using Operations.Domain.Entities.Fcl;

namespace Operations.Application.Operations.SearchOperations;

public sealed record OperationSearchPage(
    IReadOnlyList<Operation> Items,
    int TotalCount);
