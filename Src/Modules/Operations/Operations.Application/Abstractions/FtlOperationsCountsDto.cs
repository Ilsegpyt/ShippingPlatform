namespace Operations.Application.Abstractions;

public sealed record FtlOperationsCountsDto(
    int ActiveImportCount,
    int ActiveExportCount,
    int DeliveredCount,
    int InactiveCount);
