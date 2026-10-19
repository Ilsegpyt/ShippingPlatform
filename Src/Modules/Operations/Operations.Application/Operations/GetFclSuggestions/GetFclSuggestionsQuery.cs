
using BuildingBlocks.Application;
using MediatR;

namespace Operations.Application.Operations.GetFclSuggestions;

public sealed record GetFclSuggestionsQuery(Guid ExportContainerId)
    : IRequest<Result<IReadOnlyList<FclSuggestionDto>>>;


public sealed record FclSuggestionDto(
    Guid Id,
    Guid ImportOperationId,
    Guid ImportContainerId,
    Guid? ExportOperationId,
    Guid? ExportContainerId,
    string ContainerNumber,
    string ContainerType,
    string BookingNumber,
    string? Destination,
    DateOnly RequiredLoadingDate,
    TimeOnly RequiredLoadingTime,
    bool FreeTimeMatch,
    bool ShippingLineMatch,
    bool PodPolMatch,
    bool OffloadingLoadingTimeMatch,
    bool ContainerSizeMatch,
    bool DestinationMatch,
    int MatchCount,
    string Status,
    DateTime CreatedAtUtc,
    string MatchStatus,
    IReadOnlyList<string> FailedCriteria);
