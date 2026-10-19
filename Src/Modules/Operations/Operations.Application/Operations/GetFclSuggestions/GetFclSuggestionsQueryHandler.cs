
using BuildingBlocks.Application;
using MediatR;
using Operations.Application.Abstractions;

namespace Operations.Application.Operations.GetFclSuggestions;

public sealed class GetFclSuggestionsQueryHandler(
    IFclRepository repository)
    : IRequestHandler<
        GetFclSuggestionsQuery,
        Result<IReadOnlyList<FclSuggestionDto>>>
{
    public async Task<Result<IReadOnlyList<FclSuggestionDto>>> Handle(
        GetFclSuggestionsQuery query,
        CancellationToken cancellationToken)
    {
        if (query.ExportContainerId == Guid.Empty)
        {
            return Result.Failure<IReadOnlyList<FclSuggestionDto>>(
                "Export container ID is required.");
        }

        var suggestions =
            await repository.GetSuggestionsForExportContainerAsync(
                query.ExportContainerId,
                cancellationToken);

        IReadOnlyList<FclSuggestionDto> response = suggestions
            .OrderByDescending(x => x.MatchCount)
            .ThenBy(x => x.CreatedAtUtc)
            .Select(x =>
            {
                var failedCriteria = new List<string>();

                if (!x.FreeTimeMatch)
                    failedCriteria.Add("FreeTime");

                if (!x.ShippingLineMatch)
                    failedCriteria.Add("ShippingLine");

                if (!x.PodPolMatch)
                    failedCriteria.Add("POD/POL");

                if (!x.OffloadingLoadingTimeMatch)
                    failedCriteria.Add("Offloading/Loading Time");

                if (!x.ContainerSizeMatch)
                    failedCriteria.Add("Container Size");

                if (!x.DestinationMatch)
                    failedCriteria.Add("Destination");

                var matchStatus = x.MatchCount == 6
                    ? "FullMatch"
                    : "NearMatch";

                return new FclSuggestionDto(
                    x.Id,
                    x.ImportOperationId,
                    x.ImportContainerId,
                    x.ExportOperationId,
                    x.ExportContainerId,
                    x.ContainerNumber,
                    x.ContainerType.ToString(),
                    x.BookingNumber,
                    x.Destination,
                    x.RequiredLoadingDate,
                    x.RequiredLoadingTime,
                    x.FreeTimeMatch,
                    x.ShippingLineMatch,
                    x.PodPolMatch,
                    x.OffloadingLoadingTimeMatch,
                    x.ContainerSizeMatch,
                    x.DestinationMatch,
                    x.MatchCount,
                    x.Status.ToString(),
                    x.CreatedAtUtc,
                    matchStatus,
                    failedCriteria);
            })
            .ToList();

        return Result.Success(response);
    }
}
