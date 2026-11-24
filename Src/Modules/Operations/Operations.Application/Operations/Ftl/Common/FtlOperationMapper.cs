using System.Text.Json;
using Operations.Domain.Entities.Ftl;
using Operations.Domain.Enums;

namespace Operations.Application.Operations.Ftl.Common;

public static class FtlOperationMapper
{
    public static FtlOperationDto MapToDto(FtlOperation op)
    {
        var unmergedList = new List<string>();
        int? per = null;
        int? shp = null;
        int? trk = null;
        List<List<int>>? shpSpans = null;
        List<List<int>>? trkSpans = null;
        List<int>? trkOf = null;
        object? allocations = null;

        if (!string.IsNullOrWhiteSpace(op.LayoutJson))
        {
            try
            {
                using var doc = JsonDocument.Parse(op.LayoutJson);
                if (doc.RootElement.ValueKind == JsonValueKind.Array)
                {
                    var parsed = JsonSerializer.Deserialize<List<string>>(op.LayoutJson);
                    if (parsed is not null) unmergedList = parsed;
                }
                else if (doc.RootElement.ValueKind == JsonValueKind.Object)
                {
                    if (doc.RootElement.TryGetProperty("unmerged", out var uEl))
                        unmergedList = JsonSerializer.Deserialize<List<string>>(uEl.GetRawText()) ?? new();
                    if (doc.RootElement.TryGetProperty("per", out var pEl) && pEl.TryGetInt32(out var pVal))
                        per = pVal;
                    if (doc.RootElement.TryGetProperty("shp", out var sEl) && sEl.TryGetInt32(out var sVal))
                        shp = sVal;
                    if (doc.RootElement.TryGetProperty("trk", out var tEl) && tEl.TryGetInt32(out var tVal))
                        trk = tVal;
                    if (doc.RootElement.TryGetProperty("shpSpans", out var ssEl))
                        shpSpans = JsonSerializer.Deserialize<List<List<int>>>(ssEl.GetRawText());
                    if (doc.RootElement.TryGetProperty("trkSpans", out var tsEl))
                        trkSpans = JsonSerializer.Deserialize<List<List<int>>>(tsEl.GetRawText());
                    if (doc.RootElement.TryGetProperty("trkOf", out var toEl))
                        trkOf = JsonSerializer.Deserialize<List<int>>(toEl.GetRawText());
                    if (doc.RootElement.TryGetProperty("allocations", out var alEl))
                        allocations = JsonSerializer.Deserialize<object>(alEl.GetRawText());
                }
            }
            catch { /* Ignore invalid JSON */ }
        }

        var enteredBy = new FtlUserDto(
            op.CreatedByUserId,
            op.CreatedByName ?? "System");

        var tripDtos = new List<FtlTripDto>();

        var orderedTrips = op.Trips.OrderBy(t => t.Ordinal).ToList();
        var orderedShipments = op.Shipments.OrderBy(s => s.Ordinal).ToList();
        var orderedTrucks = op.Trucks.OrderBy(t => t.Ordinal).ToList();

        if (orderedTrips.Count == 0)
        {
            var defaultTripFields = BuildFallbackFields(op, null, orderedShipments.FirstOrDefault(), orderedTrucks.FirstOrDefault(), shp ?? op.NumberOfShipments);
            tripDtos.Add(new FtlTripDto(
                Guid.NewGuid().ToString(),
                0,
                defaultTripFields,
                new List<FtlUpdateDto>(),
                "Full",
                null,
                null,
                null));
        }
        else
        {
            foreach (var trip in orderedTrips)
            {
                var tripId = !string.IsNullOrWhiteSpace(trip.ExternalId)
                    ? trip.ExternalId
                    : trip.Id.ToString();

                short truckOrd = (short)(trkOf is not null && trip.Ordinal < trkOf.Count ? trkOf[trip.Ordinal] : (trip.Ordinal + 1));
                var truck = trip.Truck
                    ?? (trip.FtlTruckId.HasValue ? op.Trucks.FirstOrDefault(t => t.Id == trip.FtlTruckId.Value) : null)
                    ?? op.Trucks.FirstOrDefault(t => t.Ordinal == truckOrd);

                int shpIdx = -1;
                if (shpSpans is not null)
                {
                    shpIdx = shpSpans.FindIndex(span => span.Count >= 2 && trip.Ordinal >= span[0] && trip.Ordinal < span[0] + span[1]);
                }
                var shipment = trip.Shipment
                    ?? (trip.FtlShipmentId.HasValue ? op.Shipments.FirstOrDefault(s => s.Id == trip.FtlShipmentId.Value) : null)
                    ?? (shpIdx >= 0 && shpIdx < orderedShipments.Count ? orderedShipments[shpIdx] : orderedShipments.FirstOrDefault());

                var fields = BuildFallbackFields(op, trip, shipment, truck, shp ?? op.NumberOfShipments);

                fields["status"] = op.Status.ToString();
                fields["movement"] = op.OperationType.ToString();
                fields["vol"] = (shp ?? (op.NumberOfShipments > 0 ? op.NumberOfShipments : (op.Shipments.Count > 0 ? op.Shipments.Count : 1))).ToString();
                fields["loadStatus"] = trip.LoadStatus.ToString();
                if (trip.LoadedPackages.HasValue) fields["loadedPackages"] = trip.LoadedPackages.Value.ToString();
                if (trip.LoadedWeight.HasValue) fields["loadedWeight"] = trip.LoadedWeight.Value.ToString();
                if (trip.LoadedCbm.HasValue) fields["loadedCbm"] = trip.LoadedCbm.Value.ToString();

                var updateDtos = new List<FtlUpdateDto>();

                var rawUpdates = truck?.Updates ?? (IEnumerable<FtlTruckUpdate>)Array.Empty<FtlTruckUpdate>();

                var filteredUpdates = rawUpdates.Where(u =>
                    string.IsNullOrWhiteSpace(u.TripId) ||
                    u.TripId == tripId ||
                    u.TripId == trip.Id.ToString());

                var seenUpdates = new HashSet<string>();
                foreach (var u in filteredUpdates.OrderBy(x => x.CreatedAtUtc))
                {
                    var uKey = $"{u.CreatedAtUtc:yyyyMMddHHmmss}_{u.Note?.Trim()}";
                    if (!seenUpdates.Add(uKey)) continue;

                    updateDtos.Add(new FtlUpdateDto(
                        u.Id.ToString(),
                        u.Note,
                        u.CreatedAtUtc.ToString("HH:mm"),
                        u.CreatedByName,
                        u.CreatedAtUtc.ToString("yyyy-MM-dd"),
                        u.CreatedByName,
                        u.Note));
                }

                if (updateDtos.Count > 0)
                {
                    var lastU = updateDtos.Last();
                    fields["updates"] = lastU.Text ?? lastU.Note ?? "";
                    fields["updDate"] = $"{lastU.Day} {lastU.At}".Trim();
                }
                else if (fields.TryGetValue("updates", out var fallbackText) && !string.IsNullOrWhiteSpace(fallbackText))
                {
                    var updDateStr = fields.GetValueOrDefault("updDate") ?? "";
                    var dayStr = updDateStr.Length >= 10 ? updDateStr[..10] : op.CreatedAtUtc.ToString("yyyy-MM-dd");
                    var timeStr = updDateStr.Length > 10 ? updDateStr[11..].Trim() : op.CreatedAtUtc.ToString("HH:mm");
                    updateDtos.Add(new FtlUpdateDto(
                        Guid.NewGuid().ToString(),
                        fallbackText,
                        timeStr,
                        op.CreatedByName ?? "User",
                        dayStr,
                        op.CreatedByName ?? "User",
                        fallbackText));
                }

                tripDtos.Add(new FtlTripDto(
                    tripId,
                    trip.Ordinal,
                    fields,
                    updateDtos,
                    trip.LoadStatus.ToString(),
                    trip.LoadedPackages,
                    trip.LoadedWeight,
                    trip.LoadedCbm));
            }
        }

        var delAudit = op.StatusAudits
            .OrderByDescending(a => a.AtUtc)
            .FirstOrDefault(a => a.AfterStatus == OperationStatus.Delivered);

        var reopenAudit = op.StatusAudits
            .OrderByDescending(a => a.AtUtc)
            .FirstOrDefault(a => a.BeforeStatus == OperationStatus.Delivered && a.AfterStatus == OperationStatus.Open);

        return new FtlOperationDto(
            op.Ref,
            op.OperationType.ToString(),
            op.Status.ToString(),
            op.CreatedAtUtc.ToString("yyyy-MM-dd"),
            op.CreatedAtUtc.ToString("HH:mm"),
            enteredBy,
            unmergedList,
            tripDtos,
            per,
            shp ?? op.NumberOfShipments,
            trk,
            shpSpans,
            trkSpans,
            trkOf,
            Id: op.Id,
            IsDeleted: op.IsDeleted,
            DeletedAtUtc: op.DeletedAtUtc,
            DeletedByUserId: op.DeletedByUserId?.ToString(),
            DeletedByName: op.DeletedByName,
            DeliveredAtUtc: op.DeliveredAtUtc ?? delAudit?.AtUtc,
            DeliveredByUserId: op.DeliveredByUserId ?? delAudit?.UserId,
            DeliveredByName: op.DeliveredByName ?? (delAudit != null ? (delAudit.UserId == op.CreatedByUserId ? op.CreatedByName : delAudit.UserId) : null),
            ReopenedAtUtc: reopenAudit?.AtUtc,
            ReopenedByUserId: reopenAudit?.UserId,
            ReopenReason: null,
            TotalTrucks: op.Trucks.Count,
            TotalShipments: op.Shipments.Count,
            Allocations: allocations,
            LockedByUserId: op.LockedByUserId,
            LockedByName: op.LockedByName,
            LockedAtUtc: op.LockedAtUtc,
            IsLocked: op.IsLockActive(DateTime.UtcNow, 10));
    }

    public static Dictionary<string, string> BuildFallbackFields(
        FtlOperation op,
        FtlOperationTrip? trip,
        FtlShipment? shipment,
        FtlTruck? truck,
        int? shp = null)
    {
        var dict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        dict["day"] = op.CreatedAtUtc.ToString("yyyy-MM-dd");
        dict["at"] = op.CreatedAtUtc.ToString("HH:mm");
        dict["movement"] = op.OperationType.ToString();
        dict["status"] = op.Status.ToString();
        var volCount = shp ?? (op.NumberOfShipments > 0 ? op.NumberOfShipments : (op.Shipments.Count > 0 ? op.Shipments.Count : 1));
        dict["vol"] = volCount.ToString();

        if (shipment is not null)
        {
            if (shipment.ShipmentNumber is not null) dict["shipNo"] = shipment.ShipmentNumber;
            if (shipment.MAWB is not null)
            {
                dict["mawb"] = shipment.MAWB;
                dict["bl"] = shipment.MAWB;
                dict["booking"] = shipment.MAWB;
            }
            if (shipment.HAWB is not null) dict["hawb"] = shipment.HAWB;
            if (shipment.CertificateNumber is not null) dict["cert"] = shipment.CertificateNumber;
            if (shipment.ClearanceType.HasValue)
                dict["clrType"] = shipment.ClearanceType.Value == ClearanceType.Bosla ? "BOSLA" : "CERT";
            if (shipment.ClientName is not null) dict["client"] = shipment.ClientName;
            if (shipment.CustomsArea is not null)
            {
                dict["customsArea"] = shipment.CustomsArea;
                dict["customs"] = shipment.CustomsArea;
            }
            if (shipment.Packages.HasValue) dict["packages"] = shipment.Packages.Value.ToString();
            if (shipment.GrossWeight.HasValue) dict["weight"] = shipment.GrossWeight.Value.ToString();
            if (shipment.Cbm.HasValue) dict["cbm"] = shipment.Cbm.Value.ToString();
            if (shipment.Commodity is not null) dict["commodity"] = shipment.Commodity;
            if (shipment.Terminal is not null) dict["line"] = shipment.Terminal;
            if (shipment.Destination is not null) dict["dest"] = shipment.Destination;
            if (shipment.FullAddress is not null) dict["fullAddress"] = shipment.FullAddress;
            if (shipment.AOD is not null) dict["aod"] = shipment.AOD;
            if (shipment.InvoicesReceivedDate.HasValue) dict["invRcv"] = shipment.InvoicesReceivedDate.Value.ToString("yyyy-MM-dd");
            if (shipment.TransporterInvoiceNo is not null) dict["transInv"] = shipment.TransporterInvoiceNo;
            if (shipment.MissingDocs is not null) dict["missDoc"] = shipment.MissingDocs;
            if (shipment.MissingDocsReceivedDate.HasValue) dict["missRcv"] = shipment.MissingDocsReceivedDate.Value.ToString("yyyy-MM-dd");
        }

        if (truck is not null)
        {
            if (truck.TransporterName is not null) dict["trucker"] = truck.TransporterName;
            if (truck.TruckPlateNumber is not null)
            {
                dict["car"] = truck.TruckPlateNumber;
                dict["cntr"] = truck.TruckPlateNumber;
            }
            if (truck.TruckType.HasValue)
            {
                var ttStr = truck.TruckType.ToFrontendString();
                if (!string.IsNullOrWhiteSpace(ttStr))
                    dict["truckType"] = ttStr;
            }
            if (truck.DriverName is not null) dict["driver"] = truck.DriverName;
            if (truck.DriverMobile is not null) dict["mob"] = truck.DriverMobile;

            if (truck.ArrivalDate.HasValue) dict["arrDate"] = truck.ArrivalDate.Value.ToString("yyyy-MM-dd");
            if (truck.ArrivalTime.HasValue) dict["arrTime"] = truck.ArrivalTime.Value.ToString("HH:mm");

            if (truck.RequiredArrivalDate.HasValue)
            {
                var reqDateStr = truck.RequiredArrivalDate.Value.ToString("yyyy-MM-dd");
                dict["reqArrDate"] = reqDateStr;
                dict["reqDate"] = reqDateStr;
            }

            if (truck.RequiredArrivalTime.HasValue)
            {
                var reqTimeStr = truck.RequiredArrivalTime.Value.ToString("HH:mm");
                dict["reqArrTime"] = reqTimeStr;
                dict["reqTime"] = reqTimeStr;
            }

            if (truck.GateInDate.HasValue)
            {
                dict["gateIn"] = truck.GateInDate.Value.ToString("yyyy-MM-dd");
                if (truck.GateInTime.HasValue)
                    dict["gateInT"] = truck.GateInTime.Value.ToString("HH:mm");
            }
            else if (truck.GateInTime.HasValue)
            {
                dict["gateInT"] = truck.GateInTime.Value.ToString("HH:mm");
            }
            if (truck.GateOutDate.HasValue)
            {
                dict["gateOut"] = truck.GateOutDate.Value.ToString("yyyy-MM-dd");
                if (truck.GateOutTime.HasValue)
                    dict["gateOutT"] = truck.GateOutTime.Value.ToString("HH:mm");
            }
        }

        if (trip is not null)
        {
            dict["loadStatus"] = trip.LoadStatus.ToString();
            if (trip.LoadedPackages.HasValue) dict["loadedPackages"] = trip.LoadedPackages.Value.ToString();
            if (trip.LoadedWeight.HasValue) dict["loadedWeight"] = trip.LoadedWeight.Value.ToString();
            if (trip.LoadedCbm.HasValue) dict["loadedCbm"] = trip.LoadedCbm.Value.ToString();
        }

        return dict;
    }
}
