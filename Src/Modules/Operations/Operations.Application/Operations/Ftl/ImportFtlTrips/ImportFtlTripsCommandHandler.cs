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

namespace Operations.Application.Operations.Ftl.ImportFtlTrips;

public sealed class ImportFtlTripsCommandHandler(
    IFtlOperationRepository operationRepository,
    IOperationsUnitOfWork unitOfWork,
    ICurrentUser currentUser,
    ICustomerQueries customerQueries)
    : IRequestHandler<ImportFtlTripsCommand, Result<ImportFtlTripsResult>>
{
    public async Task<Result<ImportFtlTripsResult>> Handle(
        ImportFtlTripsCommand command,
        CancellationToken ct)
    {
        if (command.Trips is null || command.Trips.Count == 0)
            return Result.Failure<ImportFtlTripsResult>("No trips provided for import.");

        var now = DateTime.UtcNow;
        var userId = currentUser.UserId != Guid.Empty ? currentUser.UserId.ToString() : "u-1";
        var userName = await currentUser.GetUserNameAsync(ct);

        IReadOnlyList<CustomerOperationOption>? customersCache = null;

        // Pre-validate uniqueness across incoming import trips
        var batchShipNos = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var batchHawbs = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var batchMawbHawbs = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var batchCertShips = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var t in command.Trips)
        {
            var f = t.Fields ?? new Dictionary<string, string>();
            var mawb = f.GetValueOrDefault("mawb") ?? f.GetValueOrDefault("bl") ?? f.GetValueOrDefault("booking");
            var hawb = f.GetValueOrDefault("hawb");
            var cert = f.GetValueOrDefault("cert");
            var shipNo = f.GetValueOrDefault("shipNo");

            if (!string.IsNullOrWhiteSpace(shipNo))
            {
                var sNorm = IdentifierNormalizer.Normalize(shipNo);
                if (sNorm is not null)
                {
                    if (!batchShipNos.Add(sNorm))
                        return Result.Failure<ImportFtlTripsResult>($"رقم الشحنة '{shipNo.Trim()}' مكرر داخل ملف الاستيراد.");
                    if (await operationRepository.ShipmentNumberExistsAsync(sNorm, null, ct))
                        return Result.Failure<ImportFtlTripsResult>($"رقم الشحنة '{shipNo.Trim()}' مسجل بالفعل في عملية أخرى.");
                }
            }

            if (!string.IsNullOrWhiteSpace(hawb))
            {
                var hNorm = IdentifierNormalizer.Normalize(hawb);
                if (hNorm is not null)
                {
                    if (!batchHawbs.Add(hNorm))
                        return Result.Failure<ImportFtlTripsResult>($"رقم البوليصة الفرعية (HAWB) '{hawb.Trim()}' مكرر داخل ملف الاستيراد.");
                    if (await operationRepository.HawbExistsAsync(hNorm, null, ct))
                        return Result.Failure<ImportFtlTripsResult>($"رقم البوليصة الفرعية (HAWB) '{hawb.Trim()}' مسجل بالفعل في عملية أخرى.");
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
                        return Result.Failure<ImportFtlTripsResult>($"نفس بوليصة MAWB و HAWB مكرران داخل ملف الاستيراد.");
                    if (await operationRepository.MawbHawbPairExistsAsync(mNorm, hNorm, null, ct))
                        return Result.Failure<ImportFtlTripsResult>($"نفس بوليصة MAWB '{mawb.Trim()}' و HAWB '{hawb.Trim()}' مسجلان معاً بالفعل في عملية أخرى.");
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
                        return Result.Failure<ImportFtlTripsResult>($"رقم الشهادة ورقم الشحنة مكرران داخل ملف الاستيراد.");
                    if (await operationRepository.CertShipmentPairExistsAsync(cNorm, sNorm, null, ct))
                        return Result.Failure<ImportFtlTripsResult>($"رقم الشهادة '{cert.Trim()}' مع رقم الشحنة '{shipNo.Trim()}' مسجلان معاً بالفعل في عملية أخرى.");
                }
            }
        }

        // Group rows by Direction and Group (or MAWB)
        var groups = new Dictionary<string, List<ImportFtlTripRow>>();
        foreach (var t in command.Trips)
        {
            var grp = !string.IsNullOrWhiteSpace(t.Group)
                ? t.Group.Trim()
                : (t.Fields.GetValueOrDefault("mawb") ?? t.Fields.GetValueOrDefault("bl") ?? t.Fields.GetValueOrDefault("booking") ?? Guid.NewGuid().ToString());

            var key = $"{t.Direction}|{grp}";
            if (!groups.TryGetValue(key, out var list))
            {
                list = new List<ImportFtlTripRow>();
                groups[key] = list;
            }
            list.Add(t);
        }

        foreach (var (key, rows) in groups)
        {
            var isExport = string.Equals(rows[0].Direction, "Export", StringComparison.OrdinalIgnoreCase);
            var opType = isExport ? OperationType.Export : OperationType.Import;

            var opGuid = Guid.NewGuid();
            var opRef = $"OPS-{opGuid.ToString()[..8].ToUpperInvariant()}";

            var firstFields = rows[0].Fields;
            var client = firstFields.GetValueOrDefault("client");
            Guid? clientId = null;
            if (firstFields.TryGetValue("clientId", out var cIdStr) && Guid.TryParse(cIdStr, out var cGuid))
            {
                clientId = cGuid;
            }
            else if (!string.IsNullOrWhiteSpace(client))
            {
                try
                {
                    customersCache ??= await customerQueries.GetAllForOperationsAsync(ct);
                    var trimmed = client.Trim();
                    var match = customersCache.FirstOrDefault(c => string.Equals(c.CompanyName?.Trim(), trimmed, StringComparison.OrdinalIgnoreCase))
                        ?? customersCache.FirstOrDefault(c => c.CompanyName != null && (c.CompanyName.Trim().StartsWith(trimmed, StringComparison.OrdinalIgnoreCase) || trimmed.StartsWith(c.CompanyName.Trim(), StringComparison.OrdinalIgnoreCase)));
                    clientId = match?.CustomerId;
                }
                catch { /* ignore customer lookup failure */ }
            }

            var shpSpans = new List<List<int>>();
            var trkSpans = new List<List<int>>();
            var trkOf = new List<int>();

            for (int i = 0; i < rows.Count; i++)
            {
                shpSpans.Add(new List<int> { i, 1 });
                trkSpans.Add(new List<int> { i, 1 });
                trkOf.Add(i + 1);
            }

            var layoutDict = new Dictionary<string, object?>
            {
                ["unmerged"] = new List<string>(),
                ["per"] = rows.Count,
                ["shp"] = rows.Count,
                ["trk"] = rows.Count,
                ["shpSpans"] = shpSpans,
                ["trkSpans"] = trkSpans,
                ["trkOf"] = trkOf
            };

            var operation = new FtlOperation(
                opGuid,
                opType,
                rows.Count,
                JsonSerializer.Serialize(layoutDict));

            operation.SetCreatedAudit(userId, userName, now);

            short ord = 0;
            foreach (var r in rows)
            {
                var fields = new Dictionary<string, string>(r.Fields, StringComparer.OrdinalIgnoreCase);
                FtlNormalizationHelper.NormalizeTripFields(fields);

                var mawb = fields.GetValueOrDefault("mawb") ?? fields.GetValueOrDefault("bl") ?? fields.GetValueOrDefault("booking");
                var hawb = fields.GetValueOrDefault("hawb");
                var cert = fields.GetValueOrDefault("cert");
                var shipNo = fields.GetValueOrDefault("shipNo");
                var commodity = fields.GetValueOrDefault("commodity");
                var line = fields.GetValueOrDefault("line");
                var dest = fields.GetValueOrDefault("dest");
                var aod = fields.GetValueOrDefault("aod") ?? fields.GetValueOrDefault("pod");
                var customsArea = fields.GetValueOrDefault("customsArea") ?? fields.GetValueOrDefault("customs");
                var transInv = fields.GetValueOrDefault("transInv");
                var missDoc = fields.GetValueOrDefault("missDoc");

                int.TryParse(fields.GetValueOrDefault("packages"), out var packages);
                decimal.TryParse(fields.GetValueOrDefault("weight"), out var weight);
                decimal.TryParse(fields.GetValueOrDefault("cbm"), out var cbm);

                DateOnly? arrDate = DateOnly.TryParse(fields.GetValueOrDefault("arrDate"), out var ad) ? ad : null;
                TimeOnly? arrTime = TimeOnly.TryParse(fields.GetValueOrDefault("arrTime"), out var at) ? at : null;
                DateOnly? reqArrDate = DateOnly.TryParse(fields.GetValueOrDefault("reqDate") ?? fields.GetValueOrDefault("reqArrDate"), out var rd) ? rd : null;
                TimeOnly? reqArrTime = TimeOnly.TryParse(fields.GetValueOrDefault("reqTime") ?? fields.GetValueOrDefault("reqArrTime"), out var rt) ? rt : null;
                DateOnly? invRcv = DateOnly.TryParse(fields.GetValueOrDefault("invRcv"), out var ir) ? ir : null;
                DateOnly? missRcv = DateOnly.TryParse(fields.GetValueOrDefault("missRcv"), out var mr) ? mr : null;

                ClearanceType? clrType = null;
                if (Enum.TryParse<ClearanceType>(fields.GetValueOrDefault("clrType"), true, out var ctParsed))
                    clrType = ctParsed;

                var shipment = new FtlShipment(
                    operation.Id,
                    ord,
                    shipNo,
                    mawb,
                    hawb,
                    cert,
                    clrType,
                    clientId,
                    client,
                    customsArea,
                    packages > 0 ? packages : null,
                    weight > 0 ? weight : null,
                    cbm > 0 ? cbm : null,
                    commodity,
                    line,
                    dest,
                    fields.GetValueOrDefault("fullAddress") ?? fields.GetValueOrDefault("address"),
                    aod,
                    invRcv,
                    transInv,
                    missDoc,
                    missRcv);

                operation.Shipments.Add(shipment);

                var trucker = fields.GetValueOrDefault("trucker");
                var plate = fields.GetValueOrDefault("car") ?? fields.GetValueOrDefault("cntr");
                var driver = fields.GetValueOrDefault("driver");
                var mob = fields.GetValueOrDefault("mob");

                var truckType = FtlTruckTypeExtensions.ParseFtlTruckType(fields.GetValueOrDefault("truckType"));

                DateOnly? gateInDate = null;
                TimeOnly? gateInTime = null;
                if (fields.TryGetValue("gateIn", out var giStr) && !string.IsNullOrWhiteSpace(giStr))
                {
                    var parts = giStr.Trim().Split(' ');
                    if (DateOnly.TryParse(parts[0], out var gid)) gateInDate = gid;
                    if (parts.Length > 1 && TimeOnly.TryParse(parts[1], out var git)) gateInTime = git;
                }
                DateOnly? gateOutDate = DateOnly.TryParse(fields.GetValueOrDefault("gateOut"), out var god) ? god : null;
                TimeOnly? gateOutTime = TimeOnly.TryParse(fields.GetValueOrDefault("gateOutT"), out var got) ? got : null;

                var truck = new FtlTruck(
                    operation.Id,
                    (short)(ord + 1),
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

                operation.Trucks.Add(truck);

                var tripGuid = Guid.NewGuid();
                var trip = new FtlOperationTrip(
                    tripGuid,
                    operation.Id,
                    ord,
                    tripGuid.ToString(),
                    shipment.Id,
                    truck.Id,
                    FtlLoadStatus.Full,
                    packages > 0 ? packages : null,
                    weight > 0 ? weight : null,
                    cbm > 0 ? cbm : null);

                operation.Trips.Add(trip);
                ord++;
            }

            await operationRepository.AddAsync(operation, ct);
        }

        await unitOfWork.SaveChangesAsync(ct);
        return Result.Success(new ImportFtlTripsResult());
    }
}
