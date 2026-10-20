using BuildingBlocks.Application;
using MediatR;
using Operations.Application.Abstractions;
using Operations.Domain.Entities;
using Operations.Domain.Enums;

namespace Operations.Application.Operations.GenerateFclSuggestions;

public sealed class GenerateFclSuggestionsCommandHandler(
    IFclRepository repository,
    IOperationsUnitOfWork unitOfWork)
    : IRequestHandler<GenerateFclSuggestionsCommand, Result<int>>
{
    public async Task<Result<int>> Handle(
        GenerateFclSuggestionsCommand command,
        CancellationToken ct)
    {
        var exportContainer =
            await repository.GetExportContainerForMatchingAsync(
                command.ExportContainerId, ct);

        if (exportContainer?.Operation is not { } export
            || export.ExportDetails is null)
        {
            return Result.Failure<int>(
                "Open Export container with Export details was not found.");
        }

        if (!exportContainer.RequiredLoadingDate.HasValue
            || !exportContainer.RequiredLoadingTime.HasValue
            || !exportContainer.ContainerType.HasValue)
        {
            return Result.Failure<int>(
                "The selected Export container needs loading date, loading time and container type.");
        }

        var imports = await repository.GetImportCandidatesAsync(
            export.ClientId, ct);

        var existingSuggestions =
            await repository.GetSuggestionsForExportContainerAsync(
                exportContainer.Id, ct);

        // Keep approved suggestions and never recreate their Import container pair.
        var approvedImportContainers = existingSuggestions
            .Where(x => x.Status == FclSuggestionStatus.Approved)
            .Select(x => x.ImportContainerId)
            .ToHashSet();

        // All non-approved suggestions for this Export container will be rebuilt.
        var suggestionsToRemove = existingSuggestions
            .Where(x => x.Status != FclSuggestionStatus.Approved)
            .ToList();

        var suggestionsToCreate = new List<FclSuggestion>();

        var loadingAt = exportContainer.RequiredLoadingDate.Value
            .ToDateTime(exportContainer.RequiredLoadingTime.Value);

        foreach (var import in imports)
        {
            if (import.ImportDetails is not { } importDetails)
                continue;

            if (importDetails.RequiredOffloadingDate == default)
                continue;

            var offloadAt = importDetails.RequiredOffloadingDate
                .ToDateTime(importDetails.RequiredOffloadingTime);

            foreach (var importContainer in import.Containers)
            {
                if (string.IsNullOrWhiteSpace(importContainer.ContainerNumber)
                    || !importContainer.ContainerType.HasValue)
                {
                    continue;
                }

                if (approvedImportContainers.Contains(importContainer.Id))
                    continue;

                // 1. Import container free time >= Export loading + 48 hours.
                var freeTimeMatch =
                    importContainer.FreeTimeTill.HasValue
                    && importContainer.FreeTimeTill.Value
                        >= DateOnly.FromDateTime(loadingAt.AddHours(48));

                // 2. Same shipping line.
                var shippingLineMatch =
                    import.ShippingLineId == export.ShippingLineId;

                // 3. Import POD = Export POL.
                var podPolMatch =
                    importDetails.PODId == export.ExportDetails.POLId;

                // 4. Import offloading <= Export loading - 24 hours.
                var offloadingLoadingMatch =
                    offloadAt <= loadingAt.AddHours(-24);

                // 5. Same container size.
                var containerSizeMatch =
                    GetSize(importContainer.ContainerType.Value)
                    == GetSize(exportContainer.ContainerType.Value);

                // 6. Same destination, compared per container.
                var destinationMatch =
                    !string.IsNullOrWhiteSpace(importContainer.Destination)
                    && !string.IsNullOrWhiteSpace(exportContainer.Destination)
                    && Normalize(importContainer.Destination)
                        == Normalize(exportContainer.Destination);

                var matchCount = new[]
                {
                    freeTimeMatch,
                    shippingLineMatch,
                    podPolMatch,
                    offloadingLoadingMatch,
                    containerSizeMatch,
                    destinationMatch
                }.Count(x => x);

                if (matchCount < 5)
                    continue;

                var details = export.ExportDetails;

                suggestionsToCreate.Add(new FclSuggestion(
                    import.Id,
                    importContainer.Id,
                    export.Id,
                    exportContainer.Id,
                    export.ClientId,
                    details.ClearanceType,
                    details.BookingNumber,
                    export.ShippingLineId,
                    details.POLId,
                    details.POWId,
                    details.CutOffDate,
                    importContainer.ContainerNumber,
                    importContainer.ContainerType.Value,
                    exportContainer.Destination,
                    exportContainer.RequiredLoadingDate.Value,
                    exportContainer.RequiredLoadingTime.Value,
                    freeTimeMatch,
                    shippingLineMatch,
                    podPolMatch,
                    offloadingLoadingMatch,
                    containerSizeMatch,
                    destinationMatch));
            }
        }

        // Delete and recreate in one transaction so a failure cannot leave the
        // Export container without its previous suggestions.
        await unitOfWork.BeginTransactionAsync(ct);

        try
        {
            repository.RemoveSuggestions(suggestionsToRemove);
            await unitOfWork.SaveChangesAsync(ct);

            if (suggestionsToCreate.Count > 0)
            {
                await repository.AddSuggestionsAsync(suggestionsToCreate, ct);
                await unitOfWork.SaveChangesAsync(ct);
            }

            await unitOfWork.CommitTransactionAsync(ct);
        }
        catch
        {
            await unitOfWork.RollbackTransactionAsync(ct);
            throw;
        }

        return Result.Success(suggestionsToCreate.Count);
    }

    private static string Normalize(string? value) =>
        (value ?? string.Empty).Trim().ToUpperInvariant();

    private static int GetSize(ContainerType type)
    {
        var name = type.ToString();

        if (name.Contains("45", StringComparison.Ordinal))
            return 45;

        if (name.Contains("40", StringComparison.Ordinal))
            return 40;

        if (name.Contains("20", StringComparison.Ordinal))
            return 20;

        return -1;
    }
}
