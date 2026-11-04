namespace Operations.Application.Abstractions;

public sealed record OperationsCountsDto(
    int ActiveImportCount,
    int ActiveExportCount,
    int DeliveredCount,
    int InactiveCount);

