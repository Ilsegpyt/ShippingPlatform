using System.Text.Json;
using BuildingBlocks.Application;
using BuildingBlocks.Application.Abstractions;
using MediatR;
using Operations.Application.Abstractions;
using Operations.Application.Operations.Ftl.Common;
using Operations.Domain.Entities;
using Operations.Domain.Enums;

namespace Operations.Application.Operations.Ftl.UpdateFtlOperation;

public sealed class UpdateFtlOperationCommandHandler(
    IFtlOperationRepository operationRepository,
    IOperationsUnitOfWork unitOfWork,
    ICurrentUser currentUser)
    : IRequestHandler<UpdateFtlOperationCommand, Result<UpdateFtlOperationResult>>
{
    public async Task<Result<UpdateFtlOperationResult>> Handle(
        UpdateFtlOperationCommand command,
        CancellationToken ct)
    {
        var currentVersion = await operationRepository.GetCurrentWorkspaceVersionAsync(ct);

        if (command.Version.HasValue && command.Version.Value != currentVersion)
        {
            return Result.Failure<UpdateFtlOperationResult>(
                "Data changed on the server. Please reload the operation.");
        }

        var userName = await currentUser.GetUserNameAsync(ct);
        var userId = currentUser.UserId != Guid.Empty ? currentUser.UserId.ToString() : "u-1";
        var now = DateTime.UtcNow;

        var existingOperations = await operationRepository.GetAllActiveWithDetailsAsync(ct);
        var existingByRef = existingOperations.ToDictionary(x => x.Ref.ToUpperInvariant(), x => x);

        foreach (var s in command.Shipments)
        {
            var opRef = s.Ref?.Trim().ToUpperInvariant() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(opRef)) continue;

            var layoutDict = new Dictionary<string, object?>
            {
                ["unmerged"] = s.Unmerged ?? new List<string>(),
                ["per"] = s.Per ?? 1,
                ["shp"] = s.Shp ?? (s.ShpSpans?.Count ?? 1),
                ["trk"] = s.Trk ?? (s.TrkSpans?.Count ?? 1),
                ["shpSpans"] = s.ShpSpans,
                ["trkSpans"] = s.TrkSpans,
                ["trkOf"] = s.TrkOf
            };
            var layoutJson = JsonSerializer.Serialize(layoutDict);

            if (existingByRef.TryGetValue(opRef, out var existingOp))
            {
                existingOp.SetLayoutJson(layoutJson);

                var existingTripsByExtId = existingOp.Trips
                    .Where(t => !string.IsNullOrWhiteSpace(t.ExternalId))
                    .ToDictionary(t => t.ExternalId!, t => t);

                var incomingExtIds = new HashSet<string>();

                short ord = 0;
                foreach (var incomingTrip in s.Trips)
                {
                    var extId = !string.IsNullOrWhiteSpace(incomingTrip.Id)
                        ? incomingTrip.Id
                        : Guid.NewGuid().ToString();

                    incomingExtIds.Add(extId);

                    var fields = new Dictionary<string, string>(incomingTrip.Fields, StringComparer.OrdinalIgnoreCase);

                    int? tripPackages = incomingTrip.LoadedPackages;
                    if (!tripPackages.HasValue && fields.TryGetValue("loadedPackages", out var lpStr) && int.TryParse(lpStr, out var lpVal))
                        tripPackages = lpVal;
                    else if (!tripPackages.HasValue && fields.TryGetValue("packages", out var pStr) && int.TryParse(pStr, out var pVal))
                        tripPackages = pVal;

                    int rowTotalPackages = 0;
                    if (fields.TryGetValue("packages", out var pTotStr) && int.TryParse(pTotStr, out var pTotVal))
                        rowTotalPackages = pTotVal;

                    var loadStatus = FtlLoadStatus.Full;
                    if (!string.IsNullOrWhiteSpace(incomingTrip.LoadStatus) && Enum.TryParse<FtlLoadStatus>(incomingTrip.LoadStatus, true, out var lsParsed))
                        loadStatus = lsParsed;
                    else if (fields.TryGetValue("loadStatus", out var lsStr) && Enum.TryParse<FtlLoadStatus>(lsStr, true, out var lsFromFields))
                        loadStatus = lsFromFields;
                    else if (rowTotalPackages > 0 && tripPackages.HasValue && tripPackages.Value < rowTotalPackages)
                        loadStatus = FtlLoadStatus.Partial;

                    decimal? tripWeight = incomingTrip.LoadedWeight;
                    if (!tripWeight.HasValue && fields.TryGetValue("loadedWeight", out var lwStr) && decimal.TryParse(lwStr, out var lwVal))
                        tripWeight = lwVal;
                    else if (!tripWeight.HasValue && fields.TryGetValue("weight", out var wStr) && decimal.TryParse(wStr, out var wVal))
                        tripWeight = wVal;

                    decimal? tripCbm = incomingTrip.LoadedCbm;
                    if (!tripCbm.HasValue && fields.TryGetValue("loadedCbm", out var lcStr) && decimal.TryParse(lcStr, out var lcVal))
                        tripCbm = lcVal;
                    else if (!tripCbm.HasValue && fields.TryGetValue("cbm", out var cStr) && decimal.TryParse(cStr, out var cVal))
                        tripCbm = cVal;

                    fields["loadStatus"] = loadStatus.ToString();
                    if (tripPackages.HasValue) fields["loadedPackages"] = tripPackages.Value.ToString();
                    if (tripWeight.HasValue) fields["loadedWeight"] = tripWeight.Value.ToString();
                    if (tripCbm.HasValue) fields["loadedCbm"] = tripCbm.Value.ToString();
                    var fieldsJson = JsonSerializer.Serialize(fields);

                    if (existingTripsByExtId.TryGetValue(extId, out var existingTrip))
                    {
                        existingTrip.Update(
                            ord++,
                            extId,
                            existingTrip.FtlShipmentId,
                            existingTrip.FtlTruckId,
                            loadStatus,
                            tripPackages ?? existingTrip.LoadedPackages,
                            tripWeight ?? existingTrip.LoadedWeight,
                            tripCbm ?? existingTrip.LoadedCbm,
                            fieldsJson);
                    }
                    else
                    {
                        var tripGuid = Guid.TryParse(incomingTrip.Id, out var g) ? g : Guid.NewGuid();
                        var newTrip = new FtlOperationTrip(
                            tripGuid,
                            existingOp.Id,
                            ord++,
                            extId,
                            null,
                            null,
                            loadStatus,
                            tripPackages,
                            tripWeight,
                            tripCbm,
                            fieldsJson);

                        existingOp.Trips.Add(newTrip);
                    }
                }

                var tripsToRemove = existingOp.Trips
                    .Where(t => !string.IsNullOrWhiteSpace(t.ExternalId) && !incomingExtIds.Contains(t.ExternalId))
                    .ToList();

                if (tripsToRemove.Count > 0)
                {
                    await operationRepository.DeleteTripsAsync(tripsToRemove, ct);
                }

                existingOp.IncrementVersion();
            }
            else
            {
                var isExport = string.Equals(s.Direction, "Export", StringComparison.OrdinalIgnoreCase);
                var opType = isExport ? OperationType.Export : OperationType.Import;

                var firstTripFields = s.Trips.FirstOrDefault()?.Fields ?? new();
                var client = firstTripFields.GetValueOrDefault("client");
                var bl = firstTripFields.GetValueOrDefault("bl");
                var booking = firstTripFields.GetValueOrDefault("booking");
                var cert = firstTripFields.GetValueOrDefault("cert");
                var commodity = firstTripFields.GetValueOrDefault("commodity");
                var line = firstTripFields.GetValueOrDefault("line");
                var dest = firstTripFields.GetValueOrDefault("dest");
                var pod = firstTripFields.GetValueOrDefault("pod") ?? firstTripFields.GetValueOrDefault("aod");
                var customsArea = firstTripFields.GetValueOrDefault("customsArea");

                int.TryParse(firstTripFields.GetValueOrDefault("packages"), out var packages);
                decimal.TryParse(firstTripFields.GetValueOrDefault("weight"), out var weight);
                decimal.TryParse(firstTripFields.GetValueOrDefault("cbm"), out var cbm);

                DateOnly? arrDate = DateOnly.TryParse(firstTripFields.GetValueOrDefault("arrDate"), out var ad) ? ad : null;
                TimeOnly? arrTime = TimeOnly.TryParse(firstTripFields.GetValueOrDefault("arrTime"), out var at) ? at : null;
                DateOnly? dayDate = DateOnly.TryParse(s.Day, out var dd) ? dd : DateOnly.FromDateTime(now);
                TimeOnly? timeVal = TimeOnly.TryParse(s.At, out var tv) ? tv : TimeOnly.FromDateTime(now);

                var year = dayDate?.Year ?? now.Year;
                var seq = await operationRepository.GetNextSequenceForYearAsync(year, ct);
                var effectiveRef = string.IsNullOrWhiteSpace(s.Ref) ? $"OPS-{year}-{seq:D4}" : s.Ref.Trim().ToUpperInvariant();

                var newOp = new FtlOperation(
                    effectiveRef,
                    year,
                    seq,
                    opType,
                    null,
                    client,
                    bl,
                    booking,
                    null,
                    cert,
                    packages > 0 ? packages : null,
                    weight > 0 ? weight : null,
                    cbm > 0 ? cbm : null,
                    commodity,
                    line,
                    dest,
                    pod,
                    arrDate,
                    arrTime,
                    null,
                    null,
                    customsArea,
                    null,
                    layoutJson,
                    dayDate,
                    timeVal,
                    userId,
                    userName);

                short ord = 0;
                foreach (var incomingTrip in s.Trips)
                {
                    var extId = !string.IsNullOrWhiteSpace(incomingTrip.Id)
                        ? incomingTrip.Id
                        : Guid.NewGuid().ToString();

                    var fields = new Dictionary<string, string>(incomingTrip.Fields, StringComparer.OrdinalIgnoreCase);

                    int.TryParse(fields.GetValueOrDefault("packages"), out var pVal);
                    int? tripPackages = incomingTrip.LoadedPackages ?? (pVal > 0 ? pVal : null);

                    var loadStatus = FtlLoadStatus.Full;
                    if (!string.IsNullOrWhiteSpace(incomingTrip.LoadStatus) && Enum.TryParse<FtlLoadStatus>(incomingTrip.LoadStatus, true, out var lsParsed))
                        loadStatus = lsParsed;
                    else if (fields.TryGetValue("loadStatus", out var lsStr) && Enum.TryParse<FtlLoadStatus>(lsStr, true, out var lsFromFields))
                        loadStatus = lsFromFields;
                    else if (pVal > 0 && tripPackages.HasValue && tripPackages.Value < pVal)
                        loadStatus = FtlLoadStatus.Partial;

                    decimal.TryParse(fields.GetValueOrDefault("weight"), out var wVal);
                    decimal? tripWeight = incomingTrip.LoadedWeight ?? (wVal > 0 ? wVal : null);

                    decimal.TryParse(fields.GetValueOrDefault("cbm"), out var cVal);
                    decimal? tripCbm = incomingTrip.LoadedCbm ?? (cVal > 0 ? cVal : null);

                    fields["loadStatus"] = loadStatus.ToString();
                    if (tripPackages.HasValue) fields["loadedPackages"] = tripPackages.Value.ToString();
                    if (tripWeight.HasValue) fields["loadedWeight"] = tripWeight.Value.ToString();
                    if (tripCbm.HasValue) fields["loadedCbm"] = tripCbm.Value.ToString();
                    var fieldsJson = JsonSerializer.Serialize(fields);

                    var tripGuid = Guid.TryParse(incomingTrip.Id, out var g) ? g : Guid.NewGuid();
                    var newTrip = new FtlOperationTrip(
                        tripGuid,
                        newOp.Id,
                        ord++,
                        extId,
                        null,
                        null,
                        loadStatus,
                        tripPackages,
                        tripWeight,
                        tripCbm,
                        fieldsJson);

                    newOp.Trips.Add(newTrip);
                }

                await operationRepository.AddAsync(newOp, ct);
            }
        }

        await unitOfWork.SaveChangesAsync(ct);
        var newVersion = await operationRepository.GetCurrentWorkspaceVersionAsync(ct);

        return Result.Success(new UpdateFtlOperationResult(newVersion));
    }
}

