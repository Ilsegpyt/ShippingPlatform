using System.Text.Json;
using BuildingBlocks.Application;
using MediatR;
using Operations.Application.Abstractions;
using Operations.Application.Operations.Ftl.Common;
using Operations.Domain.Entities;
using Operations.Domain.Enums;

namespace Operations.Application.Operations.Ftl.GetAllFtlOperations;

public sealed class GetAllFtlOperationsQueryHandler(
    IFtlOperationRepository operationRepository)
    : IRequestHandler<GetAllFtlOperationsQuery, Result<FtlWorkspaceDto>>
{
    public async Task<Result<FtlWorkspaceDto>> Handle(GetAllFtlOperationsQuery query, CancellationToken ct)
    {
        var operations = await operationRepository.GetAllActiveWithDetailsAsync(ct);
        var currentVersion = await operationRepository.GetCurrentWorkspaceVersionAsync(ct);

        var shipmentDtos = new List<FtlOperationDto>();

        foreach (var op in operations)
        {
            var unmergedList = new List<string>();
            int? per = null;
            int? shp = null;
            int? trk = null;
            List<List<int>>? shpSpans = null;
            List<List<int>>? trkSpans = null;
            List<int>? trkOf = null;

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
                    }
                }
                catch { /* Ignore invalid JSON */ }
            }

            var enteredBy = new FtlUserDto(
                op.EnteredByUserId ?? op.CreatedByUserId,
                op.EnteredByName ?? op.CreatedByName);

            var tripDtos = new List<FtlTripDto>();

            var orderedTrips = op.Trips.OrderBy(t => t.Ordinal).ToList();
            if (orderedTrips.Count == 0)
            {
                var defaultTripFields = BuildFallbackFields(op, null, null, null);
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
                    Dictionary<string, string> fields;
                    if (!string.IsNullOrWhiteSpace(trip.FieldsJson))
                    {
                        try
                        {
                            fields = JsonSerializer.Deserialize<Dictionary<string, string>>(trip.FieldsJson) ?? new();
                        }
                        catch
                        {
                            fields = BuildFallbackFields(op, trip, trip.Shipment, trip.Truck);
                        }
                    }
                    else
                    {
                        fields = BuildFallbackFields(op, trip, trip.Shipment, trip.Truck);
                    }

                    fields["status"] = op.Status.ToString();
                    fields["movement"] = op.OperationType.ToString();
                    fields["loadStatus"] = trip.LoadStatus.ToString();
                    if (trip.LoadedPackages.HasValue) fields["loadedPackages"] = trip.LoadedPackages.Value.ToString();
                    if (trip.LoadedWeight.HasValue) fields["loadedWeight"] = trip.LoadedWeight.Value.ToString();
                    if (trip.LoadedCbm.HasValue) fields["loadedCbm"] = trip.LoadedCbm.Value.ToString();

                    var updateDtos = new List<FtlUpdateDto>();
                    if (trip.Truck?.Updates is not null)
                    {
                        foreach (var u in trip.Truck.Updates.OrderBy(x => x.CreatedAtUtc))
                        {
                            updateDtos.Add(new FtlUpdateDto(
                                u.Id.ToString(),
                                u.Note,
                                u.CreatedAtUtc.ToString("yyyy-MM-dd HH:mm"),
                                u.CreatedByName));
                        }
                    }

                    var tripId = !string.IsNullOrWhiteSpace(trip.ExternalId)
                        ? trip.ExternalId
                        : trip.Id.ToString();

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

            shipmentDtos.Add(new FtlOperationDto(
                op.Ref,
                op.OperationType.ToString(),
                op.Status.ToString(),
                op.Day.ToString("yyyy-MM-dd"),
                op.Time.ToString("HH:mm"),
                enteredBy,
                unmergedList,
                tripDtos,
                per,
                shp,
                trk,
                shpSpans,
                trkSpans,
                trkOf));
        }

        var dto = new FtlWorkspaceDto(currentVersion, shipmentDtos);
        return Result.Success(dto);
    }

    private static Dictionary<string, string> BuildFallbackFields(
        FtlOperation op,
        FtlOperationTrip? trip,
        FtlShipment? shipment,
        FtlTruck? truck)
    {
        var dict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["status"] = op.Status.ToString(),
            ["movement"] = op.OperationType.ToString()
        };

        if (op.ClientName is not null) dict["client"] = op.ClientName;
        if (op.BL is not null) dict["bl"] = op.BL;
        if (op.BookingNumber is not null) dict["booking"] = op.BookingNumber;
        if (op.ClearanceType.HasValue) dict["clrType"] = op.ClearanceType.Value.ToString();
        if (op.CertificateNumber is not null) dict["cert"] = op.CertificateNumber;
        if (op.Packages.HasValue) dict["packages"] = op.Packages.Value.ToString();
        if (op.GrossWeight.HasValue) dict["weight"] = op.GrossWeight.Value.ToString();
        if (op.Cbm.HasValue) dict["cbm"] = op.Cbm.Value.ToString();
        if (op.Commodity is not null) dict["commodity"] = op.Commodity;
        if (op.Terminal is not null) dict["line"] = op.Terminal;
        if (op.Destination is not null) dict["dest"] = op.Destination;
        if (op.AOD is not null) dict["aod"] = op.AOD;
        if (op.ArrivalDate.HasValue) dict["arrDate"] = op.ArrivalDate.Value.ToString("yyyy-MM-dd");
        if (op.ArrivalTime.HasValue) dict["arrTime"] = op.ArrivalTime.Value.ToString("HH:mm");
        if (op.RequiredArrivalDate.HasValue) dict["reqArrDate"] = op.RequiredArrivalDate.Value.ToString("yyyy-MM-dd");
        if (op.RequiredArrivalTime.HasValue) dict["reqArrTime"] = op.RequiredArrivalTime.Value.ToString("HH:mm");
        if (op.CustomsArea is not null) dict["customsArea"] = op.CustomsArea;

        if (truck is not null)
        {
            if (truck.TransporterName is not null) dict["trucker"] = truck.TransporterName;
            if (truck.TruckPlateNumber is not null) dict["car"] = truck.TruckPlateNumber;
            if (truck.DriverName is not null) dict["driver"] = truck.DriverName;
            if (truck.DriverMobile is not null) dict["mob"] = truck.DriverMobile;
            if (truck.GateInDate.HasValue) dict["gateIn"] = truck.GateInDate.Value.ToString("yyyy-MM-dd") + (truck.GateInTime.HasValue ? " " + truck.GateInTime.Value.ToString("HH:mm") : "");
            if (truck.GateOutDate.HasValue) dict["gateOut"] = truck.GateOutDate.Value.ToString("yyyy-MM-dd") + (truck.GateOutTime.HasValue ? " " + truck.GateOutTime.Value.ToString("HH:mm") : "");
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

