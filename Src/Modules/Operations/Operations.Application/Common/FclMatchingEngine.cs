using Operations.Application.Operations.GetFclSuggestions;
using Operations.Domain.Entities;
using Operations.Domain.Enums;

namespace Operations.Application.Common;

public static class FclMatchingEngine
{
    public static IReadOnlyList<FclSuggestionDto> GenerateSuggestions(
        IReadOnlyList<Operation> imports,
        IReadOnlyList<OperationContainer> exportContainers,
        ISet<Guid> approvedImportContainerIds)
    {
        var suggestions = new List<FclSuggestionDto>();
        var alreadyMatchedImportContainers = new HashSet<Guid>();

        foreach (var import in imports)
        {
            if (import.ImportDetails is not { } importDetails)
                continue;

            var importOffloadAt = importDetails.RequiredOffloadingDate
                .ToDateTime(importDetails.RequiredOffloadingTime);

            foreach (var importContainer in import.Containers
                         .OrderBy(c => c.Ordinal))
            {
                if (string.IsNullOrWhiteSpace(importContainer.ContainerNumber)
                    || !importContainer.ContainerType.HasValue
                    || !importContainer.FreeTimeTill.HasValue)
                {
                    continue;
                }

                // Keep one suggestion per Import container, and never regenerate an approved container.
                if (approvedImportContainerIds.Contains(importContainer.Id)
                    || alreadyMatchedImportContainers.Contains(importContainer.Id))
                {
                    continue;
                }

                foreach (var exportContainer in exportContainers
                             .Where(c => c.Operation.ClientId == import.ClientId)
                             .OrderBy(c => c.Operation.CreatedAtUtc)
                             .ThenBy(c => c.Ordinal))
                {
                    var export = exportContainer.Operation;
                    var exportDetails = export.ExportDetails;

                    if (exportDetails is null
                        || !string.IsNullOrWhiteSpace(exportContainer.ContainerNumber)
                        || !exportContainer.RequiredLoadingDate.HasValue
                        || !exportContainer.RequiredLoadingTime.HasValue
                        || !exportContainer.ContainerType.HasValue)
                    {
                        continue;
                    }

                    var loadingAt = exportContainer.RequiredLoadingDate.Value
                        .ToDateTime(exportContainer.RequiredLoadingTime.Value);

                    // 1. Import free time till >= Export loading + 48 hours.
                    var freeTimeMatch =
                        importContainer.FreeTimeTill.Value
                        >= DateOnly.FromDateTime(loadingAt.AddHours(48));

                    // 2. Same shipping line.
                    var shippingLineMatch =
                        import.ShippingLineId == export.ShippingLineId;

                    // 3. Import POD = Export POL.
                    var podPolMatch =
                        importDetails.PODId == exportDetails.POLId;

                    // 4. Import offloading <= Export loading - 24 hours.
                    var offloadingLoadingMatch =
                        importOffloadAt <= loadingAt.AddHours(-24);

                    // 5. Same container size.
                    var containerSizeMatch =
                        GetSize(importContainer.ContainerType.Value)
                        == GetSize(exportContainer.ContainerType.Value);

                    // 6. Same destination (container-level value on both sides).
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

                    if (matchCount != 6)
                        continue;

                    double leadTimeHours = (loadingAt - importOffloadAt).TotalHours;
                    double freeTimeBufferHours = (importContainer.FreeTimeTill.Value.ToDateTime(TimeOnly.MaxValue) - loadingAt).TotalHours;

                    var dto = new FclSuggestionDto(
                        Id: importContainer.Id,
                        ImportOperationId: import.Id,
                        ImportContainerId: importContainer.Id,
                        ExportOperationId: export.Id,
                        ExportContainerId: exportContainer.Id,
                        ContainerNumber: importContainer.ContainerNumber,
                        ContainerType: importContainer.ContainerType.Value.ToString(),
                        BookingNumber: exportDetails.BookingNumber,
                        Destination: exportContainer.Destination,
                        RequiredLoadingDate: exportContainer.RequiredLoadingDate.Value,
                        RequiredLoadingTime: exportContainer.RequiredLoadingTime.Value,
                        FreeTimeMatch: freeTimeMatch,
                        ShippingLineMatch: shippingLineMatch,
                        PodPolMatch: podPolMatch,
                        OffloadingLoadingTimeMatch: offloadingLoadingMatch,
                        ContainerSizeMatch: containerSizeMatch,
                        DestinationMatch: destinationMatch,
                        MatchCount: matchCount,
                        Status: "Pending",
                        CreatedAtUtc: DateTime.UtcNow,
                        MatchStatus: "FullMatch",
                        FailedCriteria: Array.Empty<string>(),
                        ImportContainerNumber: importContainer.ContainerNumber,
                        ImportContainerType: importContainer.ContainerType.Value.ToString(),
                        ImportMbl: importDetails.MBLNumber,
                        ImportClientId: import.ClientId.ToString(),
                        ImportShippingLine: import.ShippingLine?.Name,
                        ImportPod: importDetails.POD?.Name,
                        ImportSite: importContainer.Destination,
                        RequiredOffloadingDate: importDetails.RequiredOffloadingDate,
                        RequiredOffloadingTime: importDetails.RequiredOffloadingTime,
                        FreeTimeTill: importContainer.FreeTimeTill,
                        ExportClientId: export.ClientId.ToString(),
                        ExportShippingLine: export.ShippingLine?.Name,
                        ExportPol: exportDetails.POL?.Name,
                        ExportSite: exportContainer.Destination,
                        CutOffDate: exportDetails.CutOffDate,
                        LeadTimeHours: leadTimeHours,
                        FreeTimeBufferHours: freeTimeBufferHours);

                    suggestions.Add(dto);
                    alreadyMatchedImportContainers.Add(importContainer.Id);
                    break;
                }
            }
        }

        return suggestions;
    }

    public static (OperationContainer? ExportContainer, int MatchCount) FindBestExportMatch(
        OperationContainer importContainer,
        IReadOnlyList<OperationContainer> exportContainers)
    {
        var import = importContainer.Operation;
        if (import.ImportDetails is not { } importDetails
            || string.IsNullOrWhiteSpace(importContainer.ContainerNumber)
            || !importContainer.ContainerType.HasValue
            || !importContainer.FreeTimeTill.HasValue)
        {
            return (null, 0);
        }

        var importOffloadAt = importDetails.RequiredOffloadingDate
            .ToDateTime(importDetails.RequiredOffloadingTime);

        foreach (var exportContainer in exportContainers
                     .Where(c => c.Operation.ClientId == import.ClientId)
                     .OrderBy(c => c.Operation.CreatedAtUtc)
                     .ThenBy(c => c.Ordinal))
        {
            var export = exportContainer.Operation;
            var exportDetails = export.ExportDetails;

            if (exportDetails is null
                || !string.IsNullOrWhiteSpace(exportContainer.ContainerNumber)
                || !exportContainer.RequiredLoadingDate.HasValue
                || !exportContainer.RequiredLoadingTime.HasValue
                || !exportContainer.ContainerType.HasValue)
            {
                continue;
            }

            var loadingAt = exportContainer.RequiredLoadingDate.Value
                .ToDateTime(exportContainer.RequiredLoadingTime.Value);

            var freeTimeMatch =
                importContainer.FreeTimeTill.Value
                >= DateOnly.FromDateTime(loadingAt.AddHours(48));

            var shippingLineMatch =
                import.ShippingLineId == export.ShippingLineId;

            var podPolMatch =
                importDetails.PODId == exportDetails.POLId;

            var offloadingLoadingMatch =
                importOffloadAt <= loadingAt.AddHours(-24);

            var containerSizeMatch =
                GetSize(importContainer.ContainerType.Value)
                == GetSize(exportContainer.ContainerType.Value);

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

            if (matchCount == 6)
                return (exportContainer, 6);
        }

        return (null, 0);
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
