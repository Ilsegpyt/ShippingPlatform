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

namespace Operations.Application.Operations.Ftl.CreateFtlOperation;

public sealed class CreateFtlOperationCommandHandler(
    IFtlOperationRepository operationRepository,
    IOperationsUnitOfWork unitOfWork,
    ICurrentUser currentUser,
    ICustomerQueries customerQueries)
    : IRequestHandler<CreateFtlOperationCommand, Result<CreateFtlOperationResult>>
{
    public async Task<Result<CreateFtlOperationResult>> Handle(
        CreateFtlOperationCommand command,
        CancellationToken ct)
    {
        var isExport = string.Equals(command.Direction, "Export", StringComparison.OrdinalIgnoreCase);
        var opType = isExport ? OperationType.Export : OperationType.Import;

        var now = DateTime.UtcNow;
        var opGuid = Guid.NewGuid();
        var opRef = $"OPS-{opGuid.ToString()[..8].ToUpperInvariant()}";

        var shared = command.Shared ?? new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        var client = shared.GetValueOrDefault("client");
        Guid? clientId = null;
        if (shared.TryGetValue("clientId", out var cIdStr) && Guid.TryParse(cIdStr, out var cGuid))
        {
            clientId = cGuid;
        }
        else if (!string.IsNullOrWhiteSpace(client))
        {
            try
            {
                var customers = await customerQueries.GetAllForOperationsAsync(ct);
                var trimmed = client.Trim();
                var match = customers.FirstOrDefault(c => string.Equals(c.CompanyName?.Trim(), trimmed, StringComparison.OrdinalIgnoreCase))
                    ?? customers.FirstOrDefault(c => c.CompanyName != null && (c.CompanyName.Trim().StartsWith(trimmed, StringComparison.OrdinalIgnoreCase) || trimmed.StartsWith(c.CompanyName.Trim(), StringComparison.OrdinalIgnoreCase)));
                clientId = match?.CustomerId;
            }
            catch { /* ignore customer lookup failure */ }
        }

        var mawb = shared.GetValueOrDefault("mawb") ?? shared.GetValueOrDefault("bl") ?? shared.GetValueOrDefault("booking");
        var hawb = shared.GetValueOrDefault("hawb");
        var cert = shared.GetValueOrDefault("cert");
        var shipNo = shared.GetValueOrDefault("shipNo");
        var commodity = shared.GetValueOrDefault("commodity");
        var line = shared.GetValueOrDefault("line");
        var dest = shared.GetValueOrDefault("dest");
        var pod = shared.GetValueOrDefault("aod") ?? shared.GetValueOrDefault("pod");
        var customsArea = shared.GetValueOrDefault("customsArea") ?? shared.GetValueOrDefault("customs");

        int.TryParse(shared.GetValueOrDefault("packages"), out var packages);
        decimal.TryParse(shared.GetValueOrDefault("weight"), out var weight);
        decimal.TryParse(shared.GetValueOrDefault("cbm"), out var cbm);

        DateOnly? arrDate = DateOnly.TryParse(shared.GetValueOrDefault("arrDate"), out var ad) ? ad : null;
        TimeOnly? arrTime = TimeOnly.TryParse(shared.GetValueOrDefault("arrTime"), out var at) ? at : null;
        DateOnly? reqArrDate = DateOnly.TryParse(shared.GetValueOrDefault("reqDate") ?? shared.GetValueOrDefault("reqArrDate"), out var rd) ? rd : null;
        TimeOnly? reqArrTime = TimeOnly.TryParse(shared.GetValueOrDefault("reqTime") ?? shared.GetValueOrDefault("reqArrTime"), out var rt) ? rt : null;

        ClearanceType? clrType = null;
        if (Enum.TryParse<ClearanceType>(shared.GetValueOrDefault("clrType"), true, out var ctParsed))
            clrType = ctParsed;

        var userName = await currentUser.GetUserNameAsync(ct);
        var userId = currentUser.UserId != Guid.Empty ? currentUser.UserId.ToString() : "u-1";

        var shpCount = command.Shipments
            ?? (shared.TryGetValue("vol", out var volStr) && int.TryParse(volStr, out var vVal) ? vVal : 1);
        var trkCount = command.Trucks
            ?? command.RowCount
            ?? command.ContainerCount
            ?? 1;

        if (shpCount < 1) shpCount = 1;
        if (trkCount < 1) trkCount = 1;

        var count = command.RowCount ?? Math.Max(shpCount, trkCount);
        if (count < 1) count = 1;

        var shpSpans = CalculateEvenSpans(shpCount, count);
        var trkSpans = CalculateEvenSpans(trkCount, count);
        var trkOf = CalculateTrkOfFromSpans(trkSpans, count);

        string? GetCreateSpanField(List<CreateFtlTripInput> trips, string key, params string[] altKeys)
        {
            foreach (var t in trips)
            {
                if (t.Fields == null) continue;
                if (t.Fields.TryGetValue(key, out var val) && !string.IsNullOrWhiteSpace(val))
                    return val.Trim();
                foreach (var alt in altKeys)
                {
                    if (t.Fields.TryGetValue(alt, out var altVal) && !string.IsNullOrWhiteSpace(altVal))
                        return altVal.Trim();
                }
            }
            return null;
        }

        // Pre-validate uniqueness across all shipments to be created
        var batchShipNos = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var batchHawbs = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var batchMawbHawbs = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var batchCertShips = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        for (short sOrd = 0; sOrd < shpCount; sOrd++)
        {
            var span = sOrd < shpSpans.Count ? shpSpans[sOrd] : null;
            var startRow = span != null && span.Count >= 1 ? span[0] : (int)sOrd;
            var spanLen = span != null && span.Count >= 2 ? span[1] : 1;
            var spanTrips = command.Trips != null && startRow < command.Trips.Count
                ? command.Trips.Skip(startRow).Take(spanLen).ToList()
                : new List<CreateFtlTripInput>();

            var sShipNo = GetCreateSpanField(spanTrips, "shipNo") ?? (sOrd == 0 ? shipNo : null);
            var sMawb = GetCreateSpanField(spanTrips, "mawb", "bl", "booking") ?? mawb;
            var sHawb = GetCreateSpanField(spanTrips, "hawb") ?? (sOrd == 0 ? hawb : null);
            var sCert = GetCreateSpanField(spanTrips, "cert") ?? (sOrd == 0 ? cert : null);

            if (!string.IsNullOrWhiteSpace(sShipNo))
            {
                var sNorm = IdentifierNormalizer.Normalize(sShipNo);
                if (sNorm is not null)
                {
                    if (!batchShipNos.Add(sNorm))
                        return Result.Failure<CreateFtlOperationResult>($"رقم الشحنة '{sShipNo.Trim()}' مكرر داخل نفس العملية.");
                    if (await operationRepository.ShipmentNumberExistsAsync(sNorm, null, ct))
                        return Result.Failure<CreateFtlOperationResult>($"رقم الشحنة '{sShipNo.Trim()}' مسجل بالفعل في شحنة أخرى.");
                }
            }

            if (!string.IsNullOrWhiteSpace(sHawb))
            {
                var hNorm = IdentifierNormalizer.Normalize(sHawb);
                if (hNorm is not null)
                {
                    if (!batchHawbs.Add(hNorm))
                        return Result.Failure<CreateFtlOperationResult>($"رقم البوليصة الفرعية (HAWB) '{sHawb.Trim()}' مكرر داخل نفس العملية.");
                    if (await operationRepository.HawbExistsAsync(hNorm, null, ct))
                        return Result.Failure<CreateFtlOperationResult>($"رقم البوليصة الفرعية (HAWB) '{sHawb.Trim()}' مسجل بالفعل في شحنة أخرى.");
                }
            }

            if (!string.IsNullOrWhiteSpace(sMawb) && !string.IsNullOrWhiteSpace(sHawb))
            {
                var mNorm = IdentifierNormalizer.Normalize(sMawb);
                var hNorm = IdentifierNormalizer.Normalize(sHawb);
                if (mNorm is not null && hNorm is not null)
                {
                    var pair = $"{mNorm}|{hNorm}";
                    if (!batchMawbHawbs.Add(pair))
                        return Result.Failure<CreateFtlOperationResult>($"نفس بوليصة MAWB و HAWB مكرران داخل نفس العملية.");
                    if (await operationRepository.MawbHawbPairExistsAsync(mNorm, hNorm, null, ct))
                        return Result.Failure<CreateFtlOperationResult>($"نفس بوليصة MAWB '{sMawb.Trim()}' و HAWB '{sHawb.Trim()}' مسجلان معاً بالفعل في شحنة أخرى.");
                }
            }

            if (!string.IsNullOrWhiteSpace(sCert) && !string.IsNullOrWhiteSpace(sShipNo))
            {
                var cNorm = IdentifierNormalizer.Normalize(sCert);
                var sNorm = IdentifierNormalizer.Normalize(sShipNo);
                if (cNorm is not null && sNorm is not null)
                {
                    var certPair = $"{cNorm}|{sNorm}";
                    if (!batchCertShips.Add(certPair))
                        return Result.Failure<CreateFtlOperationResult>($"رقم الشهادة ورقم الشحنة مكرران داخل نفس العملية.");
                    if (await operationRepository.CertShipmentPairExistsAsync(cNorm, sNorm, null, ct))
                        return Result.Failure<CreateFtlOperationResult>($"رقم الشهادة '{sCert.Trim()}' مع رقم الشحنة '{sShipNo.Trim()}' مسجلان معاً بالفعل في شحنة أخرى.");
                }
            }
        }

        var layoutDict = new Dictionary<string, object?>
        {
            ["unmerged"] = new List<string>(),
            ["per"] = shpSpans.Count,
            ["shp"] = shpCount,
            ["trk"] = trkCount,
            ["shpSpans"] = shpSpans,
            ["trkSpans"] = trkSpans,
            ["trkOf"] = trkOf
        };
        var initialLayoutJson = JsonSerializer.Serialize(layoutDict);

        var operation = new FtlOperation(
            opGuid,
            opType,
            shpCount,
            initialLayoutJson);

        operation.SetCreatedAudit(userId, userName, now);

        // 1. Create Shipments (Relationally)
        var createdShipments = new List<FtlShipment>();
        for (short sOrd = 0; sOrd < shpCount; sOrd++)
        {
            var span = sOrd < shpSpans.Count ? shpSpans[sOrd] : null;
            var startRow = span != null && span.Count >= 1 ? span[0] : (int)sOrd;
            var spanLen = span != null && span.Count >= 2 ? span[1] : 1;
            var spanTrips = command.Trips != null && startRow < command.Trips.Count
                ? command.Trips.Skip(startRow).Take(spanLen).ToList()
                : new List<CreateFtlTripInput>();

            var sShipNo = GetCreateSpanField(spanTrips, "shipNo") ?? (sOrd == 0 ? shipNo : null);
            var sMawb = GetCreateSpanField(spanTrips, "mawb", "bl", "booking") ?? mawb;
            var sHawb = GetCreateSpanField(spanTrips, "hawb") ?? (sOrd == 0 ? hawb : null);
            var sCert = GetCreateSpanField(spanTrips, "cert") ?? (sOrd == 0 ? cert : null);
            var sClient = GetCreateSpanField(spanTrips, "client") ?? client;
            var sCustomsArea = GetCreateSpanField(spanTrips, "customsArea", "customs") ?? customsArea;
            var sCommodity = GetCreateSpanField(spanTrips, "commodity") ?? commodity;
            var sLine = GetCreateSpanField(spanTrips, "line", "terminal") ?? line;
            var sDest = GetCreateSpanField(spanTrips, "dest", "destination") ?? dest;
            var sPod = GetCreateSpanField(spanTrips, "aod", "pod") ?? pod;

            int.TryParse(GetCreateSpanField(spanTrips, "packages"), out var sPkg);
            decimal.TryParse(GetCreateSpanField(spanTrips, "weight"), out var sWeight);
            decimal.TryParse(GetCreateSpanField(spanTrips, "cbm"), out var sCbm);

            var resolvedPackages = sPkg > 0 ? sPkg : (packages > 0 ? packages : (int?)null);
            var resolvedWeight = sWeight > 0 ? sWeight : (weight > 0 ? weight : (decimal?)null);
            var resolvedCbm = sCbm > 0 ? sCbm : (cbm > 0 ? cbm : (decimal?)null);

            ClearanceType? sClrType = clrType;
            var clrStr = GetCreateSpanField(spanTrips, "clrType");
            if (!string.IsNullOrWhiteSpace(clrStr) && Enum.TryParse<ClearanceType>(clrStr, true, out var parsedClr))
                sClrType = parsedClr;

            var shipment = new FtlShipment(
                operation.Id,
                sOrd,
                shipmentNumber: sShipNo,
                mawb: sMawb,
                hawb: sHawb,
                certificateNumber: sCert,
                clearanceType: sClrType,
                clientId: clientId,
                clientName: sClient,
                customsArea: sCustomsArea,
                packages: resolvedPackages,
                grossWeight: resolvedWeight,
                cbm: resolvedCbm,
                commodity: sCommodity,
                terminal: sLine,
                destination: sDest,
                aod: sPod);

            operation.Shipments.Add(shipment);
            createdShipments.Add(shipment);
        }

        // 2. Create Trucks (Relationally)
        var createdTrucks = new List<FtlTruck>();
        for (short tOrd = 0; tOrd < trkCount; tOrd++)
        {
            var truckOrd = (short)(tOrd + 1);
            var truckTrip = command.Trips?.FirstOrDefault(t => t.Fields != null && trkOf.Count > t.Ordinal && trkOf[t.Ordinal] == truckOrd)
                ?? command.Trips?.Skip(tOrd < trkSpans.Count ? trkSpans[tOrd][0] : 0).FirstOrDefault()
                ?? command.Trips?.FirstOrDefault();

            var tFields = truckTrip?.Fields;
            var rawTruckType = tFields?.GetValueOrDefault("truckType") ?? shared.GetValueOrDefault("truckType");
            var parsedTruckType = FtlTruckTypeExtensions.ParseFtlTruckType(rawTruckType);

            var trucker = tFields?.GetValueOrDefault("trucker") ?? shared.GetValueOrDefault("trucker");
            var plate = tFields?.GetValueOrDefault("car") ?? tFields?.GetValueOrDefault("cntr") ?? shared.GetValueOrDefault("car");
            var driver = tFields?.GetValueOrDefault("driver") ?? shared.GetValueOrDefault("driver");
            var mob = tFields?.GetValueOrDefault("mob") ?? shared.GetValueOrDefault("mob");

            DateOnly? gateInDate = null;
            TimeOnly? gateInTime = null;
            var rawGateIn = tFields?.GetValueOrDefault("gateIn") ?? shared.GetValueOrDefault("gateIn");
            if (!string.IsNullOrWhiteSpace(rawGateIn))
            {
                var parts = rawGateIn.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (DateOnly.TryParse(parts[0], out var gid)) gateInDate = gid;
                if (parts.Length > 1 && TimeOnly.TryParse(parts[1], out var git)) gateInTime = git;
            }
            var rawGateInT = tFields?.GetValueOrDefault("gateInT") ?? shared.GetValueOrDefault("gateInT");
            if (!gateInTime.HasValue && !string.IsNullOrWhiteSpace(rawGateInT) && TimeOnly.TryParse(rawGateInT, out var git2))
                gateInTime = git2;

            DateOnly? gateOutDate = null;
            TimeOnly? gateOutTime = null;
            var rawGateOut = tFields?.GetValueOrDefault("gateOut") ?? shared.GetValueOrDefault("gateOut");
            if (!string.IsNullOrWhiteSpace(rawGateOut))
            {
                var parts = rawGateOut.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (DateOnly.TryParse(parts[0], out var god)) gateOutDate = god;
                if (parts.Length > 1 && TimeOnly.TryParse(parts[1], out var got)) gateOutTime = got;
            }
            var rawGateOutT = tFields?.GetValueOrDefault("gateOutT") ?? shared.GetValueOrDefault("gateOutT");
            if (!gateOutTime.HasValue && !string.IsNullOrWhiteSpace(rawGateOutT) && TimeOnly.TryParse(rawGateOutT, out var got2))
                gateOutTime = got2;

            var truck = new FtlTruck(
                operation.Id,
                truckOrd,
                transporterName: trucker,
                truckType: parsedTruckType,
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

            operation.Trucks.Add(truck);
            createdTrucks.Add(truck);
        }

        // 3. Create Trips (Grid rows linked to Shipment & Truck)
        var tripRefs = new List<CreateFtlTripRef>();

        if (command.Trips is not null && command.Trips.Count > 0)
        {
            short ord = 0;
            foreach (var t in command.Trips)
            {
                var tripGuid = Guid.TryParse(t.Id, out var g) ? g : Guid.NewGuid();
                var extId = t.Id ?? tripGuid.ToString();

                int? tripPackages = t.LoadedPackages;
                if (!tripPackages.HasValue && t.Fields.TryGetValue("loadedPackages", out var lpStr) && int.TryParse(lpStr, out var lpVal))
                    tripPackages = lpVal;
                else if (!tripPackages.HasValue && t.Fields.TryGetValue("packages", out var pStr) && int.TryParse(pStr, out var pVal))
                    tripPackages = pVal;
                if (!tripPackages.HasValue && packages > 0) tripPackages = packages;

                var loadStatus = FtlLoadStatus.Full;
                if (!string.IsNullOrWhiteSpace(t.LoadStatus) && Enum.TryParse<FtlLoadStatus>(t.LoadStatus, true, out var lsParsed))
                    loadStatus = lsParsed;
                else if (t.Fields.TryGetValue("loadStatus", out var lsStr) && Enum.TryParse<FtlLoadStatus>(lsStr, true, out var lsFromFields))
                    loadStatus = lsFromFields;
                else if (packages > 0 && tripPackages.HasValue && tripPackages.Value < packages)
                    loadStatus = FtlLoadStatus.Partial;

                decimal? tripWeight = t.LoadedWeight;
                if (!tripWeight.HasValue && t.Fields.TryGetValue("loadedWeight", out var lwStr) && decimal.TryParse(lwStr, out var lwVal))
                    tripWeight = lwVal;
                else if (!tripWeight.HasValue && weight > 0) tripWeight = weight;

                decimal? tripCbm = t.LoadedCbm;
                if (!tripCbm.HasValue && t.Fields.TryGetValue("loadedCbm", out var lcStr) && decimal.TryParse(lcStr, out var lcVal))
                    tripCbm = lcVal;
                else if (!tripCbm.HasValue && cbm > 0) tripCbm = cbm;

                int shpIdx = shpSpans.FindIndex(span => span.Count >= 2 && ord >= span[0] && ord < span[0] + span[1]);
                var linkedShipment = shpIdx >= 0 && shpIdx < createdShipments.Count ? createdShipments[shpIdx] : createdShipments.FirstOrDefault();

                int trkNum = ord < trkOf.Count ? trkOf[ord] : (ord + 1);
                var linkedTruck = createdTrucks.FirstOrDefault(x => x.Ordinal == trkNum) ?? createdTrucks.FirstOrDefault();

                var trip = new FtlOperationTrip(
                    tripGuid,
                    operation.Id,
                    ord++,
                    extId,
                    linkedShipment?.Id,
                    linkedTruck?.Id,
                    loadStatus,
                    tripPackages,
                    tripWeight,
                    tripCbm);

                operation.Trips.Add(trip);
                tripRefs.Add(new CreateFtlTripRef(extId));
            }
        }
        else
        {
            for (short ord = 0; ord < count; ord++)
            {
                var tripGuid = Guid.NewGuid();
                var extId = tripGuid.ToString();

                int shpIdx = shpSpans.FindIndex(span => span.Count >= 2 && ord >= span[0] && ord < span[0] + span[1]);
                var linkedShipment = shpIdx >= 0 && shpIdx < createdShipments.Count ? createdShipments[shpIdx] : createdShipments.FirstOrDefault();

                int trkNum = ord < trkOf.Count ? trkOf[ord] : (ord + 1);
                var linkedTruck = createdTrucks.FirstOrDefault(x => x.Ordinal == trkNum) ?? createdTrucks.FirstOrDefault();

                var trip = new FtlOperationTrip(
                    tripGuid,
                    operation.Id,
                    ord,
                    extId,
                    linkedShipment?.Id,
                    linkedTruck?.Id,
                    FtlLoadStatus.Full,
                    packages > 0 ? packages : null,
                    weight > 0 ? weight : null,
                    cbm > 0 ? cbm : null);

                operation.Trips.Add(trip);
                tripRefs.Add(new CreateFtlTripRef(extId));
            }
        }

        await operationRepository.AddAsync(operation, ct);
        await unitOfWork.SaveChangesAsync(ct);

        var result = new CreateFtlOperationResult(
            opRef,
            now.ToString("yyyy-MM-dd"),
            now.ToString("HH:mm"),
            tripRefs);

        return Result.Success(result);
    }

    private static List<List<int>> CalculateEvenSpans(int n, int totalRows)
    {
        n = Math.Max(1, Math.Min(n, totalRows));
        var baseLen = totalRows / n;
        var rem = totalRows % n;
        var lens = new int[n];
        Array.Fill(lens, baseLen);

        if (rem > 0)
        {
            var center = (n - 1) / 2.0;
            var order = Enumerable.Range(0, n)
                .OrderBy(a => Math.Abs(a - center))
                .ThenBy(a => a)
                .ToList();

            for (var i = 0; i < rem; i++)
            {
                lens[order[i]]++;
            }
        }

        var outSpans = new List<List<int>>(n);
        var start = 0;
        for (var i = 0; i < n; i++)
        {
            outSpans.Add(new List<int> { start, lens[i] });
            start += lens[i];
        }

        return outSpans;
    }

    private static List<int> CalculateTrkOfFromSpans(List<List<int>> spans, int totalRows)
    {
        var outTrk = new int[totalRows];
        Array.Fill(outTrk, 1);
        for (var k = 0; k < spans.Count; k++)
        {
            var span = spans[k];
            var a = span[0];
            var l = span[1];
            for (var r = a; r < a + l && r < totalRows; r++)
            {
                outTrk[r] = k + 1;
            }
        }
        return outTrk.ToList();
    }
}
