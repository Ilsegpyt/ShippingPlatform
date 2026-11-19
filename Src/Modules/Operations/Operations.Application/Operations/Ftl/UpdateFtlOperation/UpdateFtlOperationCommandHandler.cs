using System.Text.Json;
using BuildingBlocks.Application;
using BuildingBlocks.Application.Abstractions;
using Customers.Contracts;
using MediatR;
using Operations.Application.Abstractions;
using Operations.Application.Operations.Ftl.Common;
using Operations.Domain.Common;
using Operations.Domain.Entities;
using Operations.Domain.Entities.Ftl;
using Operations.Domain.Enums;

namespace Operations.Application.Operations.Ftl.UpdateFtlOperation;

public sealed class UpdateFtlOperationCommandHandler(
    IFtlOperationRepository operationRepository,
    IOperationsUnitOfWork unitOfWork,
    ICurrentUser currentUser,
    ICustomerQueries customerQueries)
    : IRequestHandler<UpdateFtlOperationCommand, Result<UpdateFtlOperationResult>>
{
    public async Task<Result<UpdateFtlOperationResult>> Handle(
        UpdateFtlOperationCommand command,
        CancellationToken ct)
    {
        var userName = await currentUser.GetUserNameAsync(ct);
        var userId = currentUser.UserId != Guid.Empty ? currentUser.UserId.ToString() : "u-1";
        var now = DateTime.UtcNow;

        IReadOnlyList<CustomerOperationOption>? customersCache = null;
        async Task<Guid?> ResolveClientIdAsync(string? clientName, string? candidateClientId)
        {
            if (!string.IsNullOrWhiteSpace(candidateClientId) && Guid.TryParse(candidateClientId, out var g))
                return g;
            if (string.IsNullOrWhiteSpace(clientName))
                return null;
            try
            {
                customersCache ??= await customerQueries.GetAllForOperationsAsync(ct);
                var trimmed = clientName.Trim();
                var match = customersCache.FirstOrDefault(c => string.Equals(c.CompanyName?.Trim(), trimmed, StringComparison.OrdinalIgnoreCase))
                    ?? customersCache.FirstOrDefault(c => c.CompanyName != null && (c.CompanyName.Trim().StartsWith(trimmed, StringComparison.OrdinalIgnoreCase) || trimmed.StartsWith(c.CompanyName.Trim(), StringComparison.OrdinalIgnoreCase)));
                return match?.CustomerId;
            }
            catch { return null; }
        }

        var existingOperations = await operationRepository.GetAllActiveWithDetailsAsync(ct);
        var existingByRef = existingOperations.ToDictionary(x => x.Ref.ToUpperInvariant(), x => x);

        // Pre-validate uniqueness across incoming shipments and operations
        var batchShipNos = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var batchHawbs = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var batchMawbHawbs = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var batchCertShips = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        static string? GetSpanVal(List<FtlTripDto> rows, string key, params string[] altKeys)
        {
            foreach (var r in rows)
            {
                if (r.Fields == null) continue;
                if (r.Fields.TryGetValue(key, out var val) && !string.IsNullOrWhiteSpace(val))
                    return val.Trim();
                foreach (var alt in altKeys)
                {
                    if (r.Fields.TryGetValue(alt, out var altVal) && !string.IsNullOrWhiteSpace(altVal))
                        return altVal.Trim();
                }
            }
            return null;
        }

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
            if (s.Allocations is not null)
            {
                layoutDict["allocations"] = s.Allocations;
            }
            var layoutJson = JsonSerializer.Serialize(layoutDict);

            var isExport = string.Equals(s.Direction, "Export", StringComparison.OrdinalIgnoreCase);

            if (existingByRef.TryGetValue(opRef, out var existingOp))
            {
                var currentUserGuid = currentUser.UserId != Guid.Empty ? currentUser.UserId : (Guid?)null;
                if (existingOp.IsLockActive(now, 10))
                {
                    if (currentUserGuid.HasValue && existingOp.LockedByUserId.HasValue && existingOp.LockedByUserId.Value != currentUserGuid.Value)
                    {
                        return Result.Failure<UpdateFtlOperationResult>(
                            $"Operation '{opRef}' is currently locked by {existingOp.LockedByName}. Save halted to prevent conflict.");
                    }
                }

                existingOp.SetLayoutJson(layoutJson);

                // --- 1. Process Shipments (Relationally from ShpSpans) ---
                var shpSpans = s.ShpSpans ?? new List<List<int>> { new List<int> { 0, s.Trips.Count } };
                if (shpSpans.Count == 0) shpSpans = new List<List<int>> { new List<int> { 0, s.Trips.Count } };

                for (short shpIdx = 0; shpIdx < shpSpans.Count; shpIdx++)
                {
                    var span = shpSpans[shpIdx];
                    var startRow = span.Count >= 1 ? span[0] : 0;
                    var len = span.Count >= 2 ? span[1] : 1;
                    var spanRows = s.Trips.Skip(startRow).Take(len).ToList();

                    var mawb = GetSpanVal(spanRows, "mawb", "bl", "booking");
                    var hawb = GetSpanVal(spanRows, "hawb");
                    var cert = GetSpanVal(spanRows, "cert");
                    var shipNo = GetSpanVal(spanRows, "shipNo");

                    if (!string.IsNullOrWhiteSpace(shipNo))
                    {
                        var sNorm = IdentifierNormalizer.Normalize(shipNo);
                        if (sNorm is not null)
                        {
                            if (!batchShipNos.Add(sNorm))
                                return Result.Failure<UpdateFtlOperationResult>($"رقم الشحنة '{shipNo.Trim()}' مكرر داخل نفس العملية.");

                            // Check conflict across other active operations
                            if (await operationRepository.ShipmentNumberExistsAsync(sNorm, existingOp.Id, ct))
                                return Result.Failure<UpdateFtlOperationResult>($"رقم الشحنة '{shipNo.Trim()}' مسجل بالفعل في عملية أخرى.");
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(hawb))
                    {
                        var hNorm = IdentifierNormalizer.Normalize(hawb);
                        if (hNorm is not null)
                        {
                            if (!batchHawbs.Add(hNorm))
                                return Result.Failure<UpdateFtlOperationResult>($"رقم البوليصة الفرعية (HAWB) '{hawb.Trim()}' مكرر داخل نفس العملية.");

                            // Check conflict across other active operations
                            if (await operationRepository.HawbExistsAsync(hNorm, existingOp.Id, ct))
                                return Result.Failure<UpdateFtlOperationResult>($"رقم البوليصة الفرعية (HAWB) '{hawb.Trim()}' مسجل بالفعل في عملية أخرى.");
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(mawb) && !string.IsNullOrWhiteSpace(hawb))
                    {
                        var mNorm = IdentifierNormalizer.Normalize(mawb);
                        var hNorm = IdentifierNormalizer.Normalize(hawb);
                        if (mNorm is not null && hNorm is not null)
                        {
                            var pair = $"{mNorm}|{hNorm}";
                            if (!batchMawbHawbs.Add(pair))
                                return Result.Failure<UpdateFtlOperationResult>($"نفس بوليصة MAWB و HAWB مكرران داخل نفس العملية.");

                            // Check conflict across other active operations
                            if (await operationRepository.MawbHawbPairExistsAsync(mNorm, hNorm, existingOp.Id, ct))
                                return Result.Failure<UpdateFtlOperationResult>($"نفس بوليصة MAWB '{mawb.Trim()}' و HAWB '{hawb.Trim()}' مسجلان معاً بالفعل في عملية أخرى.");
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(cert) && !string.IsNullOrWhiteSpace(shipNo))
                    {
                        var cNorm = IdentifierNormalizer.Normalize(cert);
                        var sNorm = IdentifierNormalizer.Normalize(shipNo);
                        if (cNorm is not null && sNorm is not null)
                        {
                            var certPair = $"{cNorm}|{sNorm}";
                            if (!batchCertShips.Add(certPair))
                                return Result.Failure<UpdateFtlOperationResult>($"رقم الشهادة ورقم الشحنة مكرران داخل نفس العملية.");

                            // Check conflict across other active operations
                            if (await operationRepository.CertShipmentPairExistsAsync(cNorm, sNorm, existingOp.Id, ct))
                                return Result.Failure<UpdateFtlOperationResult>($"رقم الشهادة '{cert.Trim()}' مع رقم الشحنة '{shipNo.Trim()}' مسجلان معاً بالفعل في عملية أخرى.");
                        }
                    }
                }

                existingOp.UpdateNumberOfShipments(shpSpans.Count);

                var existingShipmentsByOrd = existingOp.Shipments.ToDictionary(sh => sh.Ordinal, sh => sh);
                var activeShipments = new List<FtlShipment>();

                for (short shpIdx = 0; shpIdx < shpSpans.Count; shpIdx++)
                {
                    var span = shpSpans[shpIdx];
                    var startRow = span.Count >= 1 ? span[0] : 0;
                    var len = span.Count >= 2 ? span[1] : 1;

                    // Combine fields across all rows in this span
                    var spanRows = s.Trips.Skip(startRow).Take(len).ToList();

                    var client = GetSpanVal(spanRows, "client");
                    var mawb = GetSpanVal(spanRows, "mawb", "bl", "booking");
                    var hawb = GetSpanVal(spanRows, "hawb");
                    var cert = GetSpanVal(spanRows, "cert");
                    var shipNo = GetSpanVal(spanRows, "shipNo");
                    var commodity = GetSpanVal(spanRows, "commodity");
                    var line = GetSpanVal(spanRows, "line", "terminal");
                    var dest = GetSpanVal(spanRows, "dest", "destination");
                    var aod = GetSpanVal(spanRows, "aod", "pod");
                    var customsArea = GetSpanVal(spanRows, "customsArea", "customs");
                    var transInv = GetSpanVal(spanRows, "transInv");
                    var missDoc = GetSpanVal(spanRows, "missDoc");

                    int.TryParse(GetSpanVal(spanRows, "packages"), out var packages);
                    decimal.TryParse(GetSpanVal(spanRows, "weight"), out var weight);
                    decimal.TryParse(GetSpanVal(spanRows, "cbm"), out var cbm);

                    DateOnly? invRcv = DateOnly.TryParse(GetSpanVal(spanRows, "invRcv"), out var ir) ? ir : null;
                    DateOnly? missRcv = DateOnly.TryParse(GetSpanVal(spanRows, "missRcv"), out var mr) ? mr : null;

                    ClearanceType? clrType = null;
                    var clrTypeStr = GetSpanVal(spanRows, "clrType");
                    if (!string.IsNullOrWhiteSpace(clrTypeStr) && Enum.TryParse<ClearanceType>(clrTypeStr, true, out var ctParsed))
                        clrType = ctParsed;

                    var resolvedClientId = await ResolveClientIdAsync(client, GetSpanVal(spanRows, "clientId"));

                    if (existingShipmentsByOrd.TryGetValue(shpIdx, out var existingShipment))
                    {
                        existingShipment.Update(
                            shipNo,
                            mawb,
                            hawb,
                            cert,
                            clrType ?? existingShipment.ClearanceType,
                            resolvedClientId ?? existingShipment.ClientId,
                            string.IsNullOrWhiteSpace(client) ? existingShipment.ClientName : client,
                            string.IsNullOrWhiteSpace(customsArea) ? existingShipment.CustomsArea : customsArea,
                            packages > 0 ? packages : existingShipment.Packages,
                            weight > 0 ? weight : existingShipment.GrossWeight,
                            cbm > 0 ? cbm : existingShipment.Cbm,
                            string.IsNullOrWhiteSpace(commodity) ? existingShipment.Commodity : commodity,
                            string.IsNullOrWhiteSpace(line) ? existingShipment.Terminal : line,
                            string.IsNullOrWhiteSpace(dest) ? existingShipment.Destination : dest,
                            string.IsNullOrWhiteSpace(aod) ? existingShipment.AOD : aod,
                            invRcv ?? existingShipment.InvoicesReceivedDate,
                            string.IsNullOrWhiteSpace(transInv) ? existingShipment.TransporterInvoiceNo : transInv,
                            string.IsNullOrWhiteSpace(missDoc) ? existingShipment.MissingDocs : missDoc,
                            missRcv ?? existingShipment.MissingDocsReceivedDate);

                        activeShipments.Add(existingShipment);
                    }
                    else
                    {
                        var newShipment = new FtlShipment(
                            existingOp.Id,
                            shpIdx,
                            shipNo,
                            mawb,
                            hawb,
                            cert,
                            clrType,
                            resolvedClientId,
                            client,
                            customsArea,
                            packages > 0 ? packages : null,
                            weight > 0 ? weight : null,
                            cbm > 0 ? cbm : null,
                            commodity,
                            line,
                            dest,
                            aod,
                            invRcv,
                            transInv,
                            missDoc,
                            missRcv);

                        existingOp.Shipments.Add(newShipment);
                        await operationRepository.AddShipmentAsync(newShipment, ct);
                        activeShipments.Add(newShipment);
                    }
                }

                // Remove excess shipments
                var excessShipments = existingOp.Shipments.Where(sh => sh.Ordinal >= shpSpans.Count).ToList();
                if (excessShipments.Count > 0)
                {
                    await operationRepository.DeleteShipmentsAsync(excessShipments, ct);
                }

                // --- 2. Process Trucks (Relationally from TrkSpans / TrkOf) ---
                var trkSpans = s.TrkSpans ?? new List<List<int>> { new List<int> { 0, s.Trips.Count } };
                int trkCount = s.Trk ?? trkSpans.Count;
                if (trkCount < 1) trkCount = 1;

                var existingTrucksByOrd = existingOp.Trucks.ToDictionary(t => t.Ordinal, t => t);
                var activeTrucks = new List<FtlTruck>();

                for (short trkIdx = 0; trkIdx < trkCount; trkIdx++)
                {
                    short truckOrd = (short)(trkIdx + 1);

                    // Find first row belonging to this truck
                    var truckTrip = s.Trips.FirstOrDefault(t => s.TrkOf is not null && t.Ordinal < s.TrkOf.Count && s.TrkOf[t.Ordinal] == truckOrd)
                        ?? s.Trips.Skip(trkIdx < trkSpans.Count ? trkSpans[trkIdx][0] : 0).FirstOrDefault()
                        ?? s.Trips.FirstOrDefault();

                    var tFields = truckTrip?.Fields ?? new Dictionary<string, string>();

                    var trucker = tFields.GetValueOrDefault("trucker");
                    var plate = tFields.GetValueOrDefault("car") ?? tFields.GetValueOrDefault("cntr");
                    var driver = tFields.GetValueOrDefault("driver");
                    var mob = tFields.GetValueOrDefault("mob");

                    FtlTruckType? truckType = null;
                    var hasTruckType = tFields.TryGetValue("truckType", out var rawTruckType);
                    if (hasTruckType)
                    {
                        truckType = FtlTruckTypeExtensions.ParseFtlTruckType(rawTruckType);
                    }

                    DateOnly? arrDate = DateOnly.TryParse(tFields.GetValueOrDefault("arrDate"), out var ad) ? ad : null;
                    TimeOnly? arrTime = TimeOnly.TryParse(tFields.GetValueOrDefault("arrTime"), out var at) ? at : null;
                    DateOnly? reqArrDate = DateOnly.TryParse(tFields.GetValueOrDefault("reqDate") ?? tFields.GetValueOrDefault("reqArrDate"), out var rd) ? rd : null;
                    TimeOnly? reqArrTime = TimeOnly.TryParse(tFields.GetValueOrDefault("reqTime") ?? tFields.GetValueOrDefault("reqArrTime"), out var rt) ? rt : null;

                    DateOnly? gateInDate = null;
                    TimeOnly? gateInTime = null;
                    if (tFields.TryGetValue("gateIn", out var giStr) && !string.IsNullOrWhiteSpace(giStr))
                    {
                        var parts = giStr.Trim().Split(' ');
                        if (DateOnly.TryParse(parts[0], out var gid)) gateInDate = gid;
                        if (parts.Length > 1 && TimeOnly.TryParse(parts[1], out var git)) gateInTime = git;
                    }
                    if (!gateInTime.HasValue && tFields.TryGetValue("gateInT", out var gitStr) && TimeOnly.TryParse(gitStr, out var git2))
                        gateInTime = git2;

                    DateOnly? gateOutDate = DateOnly.TryParse(tFields.GetValueOrDefault("gateOut"), out var god) ? god : null;
                    TimeOnly? gateOutTime = TimeOnly.TryParse(tFields.GetValueOrDefault("gateOutT"), out var got) ? got : null;

                    if (existingTrucksByOrd.TryGetValue(truckOrd, out var existingTruck))
                    {
                        existingTruck.Update(
                            existingTruck.TransporterId,
                            string.IsNullOrWhiteSpace(trucker) ? existingTruck.TransporterName : trucker,
                            hasTruckType ? truckType : existingTruck.TruckType,
                            string.IsNullOrWhiteSpace(plate) ? existingTruck.TruckPlateNumber : plate,
                            existingTruck.DriverId,
                            string.IsNullOrWhiteSpace(driver) ? existingTruck.DriverName : driver,
                            string.IsNullOrWhiteSpace(mob) ? existingTruck.DriverMobile : mob,
                            isExport ? null : (arrDate ?? existingTruck.ArrivalDate),
                            isExport ? null : (arrTime ?? existingTruck.ArrivalTime),
                            isExport ? (reqArrDate ?? existingTruck.RequiredArrivalDate) : null,
                            isExport ? (reqArrTime ?? existingTruck.RequiredArrivalTime) : null,
                            gateInDate ?? existingTruck.GateInDate,
                            gateInTime ?? existingTruck.GateInTime,
                            gateOutDate ?? existingTruck.GateOutDate,
                            gateOutTime ?? existingTruck.GateOutTime);

                        activeTrucks.Add(existingTruck);
                    }
                    else
                    {
                        var newTruck = new FtlTruck(
                            existingOp.Id,
                            truckOrd,
                            transporterName: trucker,
                            truckType: truckType,
                            truckPlateNumber: plate,
                            driverName: driver,
                            driverMobile: mob,
                            arrivalDate: isExport ? null : arrDate,
                            arrivalTime: isExport ? null : arrTime,
                            requiredArrivalDate: isExport ? reqArrDate : null,
                            requiredArrivalTime: isExport ? reqArrTime : null,
                            gateInDate: gateInDate,
                            gateInTime: gateInTime,
                            gateOutDate: gateOutDate,
                            gateOutTime: gateOutTime);

                        existingOp.Trucks.Add(newTruck);
                        await operationRepository.AddTruckAsync(newTruck, ct);
                        activeTrucks.Add(newTruck);
                    }
                }

                // Remove excess trucks
                var excessTrucks = existingOp.Trucks.Where(t => t.Ordinal > trkCount).ToList();
                if (excessTrucks.Count > 0)
                {
                    await operationRepository.DeleteTrucksAsync(excessTrucks, ct);
                }

                // --- 3. Process Trips (Relationally linked to active shipments and trucks) ---
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
                    FtlNormalizationHelper.NormalizeTripFields(fields);

                    int? tripPackages = incomingTrip.LoadedPackages;
                    if (!tripPackages.HasValue && fields.TryGetValue("loadedPackages", out var lpStr) && !string.IsNullOrWhiteSpace(lpStr) && int.TryParse(lpStr, out var lpVal))
                        tripPackages = lpVal;

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

                    int sIdx = -1;
                    for (int si = 0; si < shpSpans.Count; si++)
                    {
                        var span = shpSpans[si];
                        if (span.Count >= 2 && ord >= span[0] && ord < span[0] + span[1])
                        {
                            sIdx = si;
                            break;
                        }
                    }
                    var targetShipment = sIdx >= 0 && sIdx < activeShipments.Count ? activeShipments[sIdx] : activeShipments.FirstOrDefault();

                    short tNum = (short)(s.TrkOf is not null && ord < s.TrkOf.Count ? s.TrkOf[ord] : (ord + 1));
                    var targetTruck = activeTrucks.FirstOrDefault(t => t.Ordinal == tNum) ?? activeTrucks.FirstOrDefault();

                    if (existingTripsByExtId.TryGetValue(extId, out var existingTrip))
                    {
                        existingTrip.Update(
                            ord,
                            extId,
                            targetShipment?.Id,
                            targetTruck?.Id,
                            loadStatus,
                            tripPackages,
                            tripWeight ?? existingTrip.LoadedWeight,
                            tripCbm ?? existingTrip.LoadedCbm);
                    }
                    else
                    {
                        var tripGuid = Guid.TryParse(incomingTrip.Id, out var g) ? g : Guid.NewGuid();
                        var newTrip = new FtlOperationTrip(
                            tripGuid,
                            existingOp.Id,
                            ord,
                            extId,
                            targetShipment?.Id,
                            targetTruck?.Id,
                            loadStatus,
                            tripPackages,
                            tripWeight,
                            tripCbm);

                        existingOp.Trips.Add(newTrip);
                        await operationRepository.AddTripAsync(newTrip, ct);
                    }

                    // Handle truck updates
                    if (incomingTrip.Updates is { Count: > 0 } updatesList && targetTruck is not null)
                    {
                        var existingNotes = new HashSet<string>(targetTruck.Updates.Select(u => u.Note.Trim()), StringComparer.OrdinalIgnoreCase);
                        foreach (var u in updatesList)
                        {
                            var text = (u.Text ?? u.Note)?.Trim();
                            if (string.IsNullOrWhiteSpace(text)) continue;
                            if (!existingNotes.Contains(text))
                            {
                                DateTime uDate = now;
                                if (!string.IsNullOrWhiteSpace(u.Day) && DateOnly.TryParse(u.Day, out var dOnly))
                                {
                                    TimeOnly tOnly = TimeOnly.TryParse(u.At, out var tParsed) ? tParsed : TimeOnly.MinValue;
                                    uDate = dOnly.ToDateTime(tOnly, DateTimeKind.Utc);
                                }
                                var newUpd = targetTruck.AddUpdate(extId, text, userId, u.By ?? u.User ?? userName, uDate);
                                await operationRepository.AddTruckUpdateAsync(newUpd, ct);
                                existingNotes.Add(text);
                            }
                        }
                    }

                    ord++;
                }

                // Remove deleted trips
                var tripsToRemove = existingOp.Trips
                    .Where(t => !string.IsNullOrWhiteSpace(t.ExternalId) && !incomingExtIds.Contains(t.ExternalId))
                    .ToList();

                if (tripsToRemove.Count > 0)
                {
                    await operationRepository.DeleteTripsAsync(tripsToRemove, ct);
                }

                existingOp.ReleaseLock();
            }
        }

        try
        {
            await unitOfWork.SaveChangesAsync(ct);
        }
        catch (Exception dbEx)
        {
            var msg = dbEx.InnerException?.Message ?? dbEx.Message;
            if (msg.Contains("UX_FtlShipments_ShipmentNumber"))
                return Result.Failure<UpdateFtlOperationResult>("رقم الشحنة (Shipment Number) مسجل بالفعل لشحنة أخرى وممنوع تكراره.");
            if (msg.Contains("UX_FtlShipments_HAWB"))
                return Result.Failure<UpdateFtlOperationResult>("رقم البوليصة الفرعية (HAWB) مسجل بالفعل وممنوع تكراره.");
            if (msg.Contains("UX_FtlShipments_MAWB_HAWB"))
                return Result.Failure<UpdateFtlOperationResult>("رقم البوليصة الرئيسية (MAWB) والبوليصة الفرعية (HAWB) مسجلان معاً بالفعل في شحنة أخرى.");
            if (msg.Contains("UX_FtlShipments_Cert_ShipmentNumber"))
                return Result.Failure<UpdateFtlOperationResult>("رقم الشهادة (Certificate #) مسجل بالفعل مع نفس رقم الشحنة.");

            return Result.Failure<UpdateFtlOperationResult>($"تعذر حفظ التعديلات: {msg}");
        }

        return Result.Success(new UpdateFtlOperationResult());
    }
}
