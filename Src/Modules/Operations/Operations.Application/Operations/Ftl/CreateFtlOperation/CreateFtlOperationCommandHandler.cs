using System.Text.Json;
using BuildingBlocks.Application;
using BuildingBlocks.Application.Abstractions;
using MediatR;
using Operations.Application.Abstractions;
using Operations.Domain.Entities;
using Operations.Domain.Enums;

namespace Operations.Application.Operations.Ftl.CreateFtlOperation;

public sealed class CreateFtlOperationCommandHandler(
    IFtlOperationRepository operationRepository,
    IOperationsUnitOfWork unitOfWork,
    ICurrentUser currentUser)
    : IRequestHandler<CreateFtlOperationCommand, Result<CreateFtlOperationResult>>
{
    public async Task<Result<CreateFtlOperationResult>> Handle(
        CreateFtlOperationCommand command,
        CancellationToken ct)
    {
        var isExport = string.Equals(command.Direction, "Export", StringComparison.OrdinalIgnoreCase);
        var opType = isExport ? OperationType.Export : OperationType.Import;

        var now = DateTime.UtcNow;
        var year = now.Year;
        var seq = await operationRepository.GetNextSequenceForYearAsync(year, ct);
        var opRef = $"OPS-{year}-{seq:D4}";

        var shared = command.Shared ?? new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        var client = shared.GetValueOrDefault("client");
        var bl = shared.GetValueOrDefault("bl");
        var booking = shared.GetValueOrDefault("booking");
        var cert = shared.GetValueOrDefault("cert");
        var commodity = shared.GetValueOrDefault("commodity");
        var line = shared.GetValueOrDefault("line");
        var dest = shared.GetValueOrDefault("dest");
        var pod = shared.GetValueOrDefault("pod");
        var customsArea = shared.GetValueOrDefault("customsArea");

        int.TryParse(shared.GetValueOrDefault("packages"), out var packages);
        decimal.TryParse(shared.GetValueOrDefault("weight"), out var weight);
        decimal.TryParse(shared.GetValueOrDefault("cbm"), out var cbm);

        DateOnly? arrDate = DateOnly.TryParse(shared.GetValueOrDefault("arrDate"), out var ad) ? ad : null;
        TimeOnly? arrTime = TimeOnly.TryParse(shared.GetValueOrDefault("arrTime"), out var at) ? at : null;
        DateOnly? reqArrDate = DateOnly.TryParse(shared.GetValueOrDefault("reqArrDate"), out var rd) ? rd : null;
        TimeOnly? reqArrTime = TimeOnly.TryParse(shared.GetValueOrDefault("reqArrTime"), out var rt) ? rt : null;

        ClearanceType? clrType = null;
        if (Enum.TryParse<ClearanceType>(shared.GetValueOrDefault("clrType"), true, out var ctParsed))
            clrType = ctParsed;

        var today = DateOnly.FromDateTime(now);
        var timeNow = TimeOnly.FromDateTime(now);
        var userName = await currentUser.GetUserNameAsync(ct);
        var userId = currentUser.UserId != Guid.Empty ? currentUser.UserId.ToString() : "u-1";

        var count = command.RowCount ?? command.ContainerCount ?? 1;
        if (count < 1) count = 1;

        var layoutDict = new Dictionary<string, object?>
        {
            ["unmerged"] = new List<string>(),
            ["per"] = 1,
            ["shp"] = 1,
            ["trk"] = count,
            ["shpSpans"] = new List<List<int>> { new List<int> { 0, count } },
            ["trkSpans"] = Enumerable.Range(0, count).Select(i => new List<int> { i, 1 }).ToList(),
            ["trkOf"] = Enumerable.Range(1, count).ToList()
        };
        var initialLayoutJson = JsonSerializer.Serialize(layoutDict);

        var operation = new FtlOperation(
            opRef,
            year,
            seq,
            opType,
            null,
            client,
            bl,
            booking,
            clrType,
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
            reqArrDate,
            reqArrTime,
            customsArea,
            null,
            initialLayoutJson,
            today,
            timeNow,
            userId,
            userName);

        operation.SetCreatedAudit(userId, userName, now);

        var tripRefs = new List<CreateFtlTripRef>();

        if (command.Trips is not null && command.Trips.Count > 0)
        {
            short ord = 0;
            foreach (var t in command.Trips)
            {
                var tripGuid = Guid.TryParse(t.Id, out var g) ? g : Guid.NewGuid();
                var extId = t.Id ?? tripGuid.ToString();

                var tripFields = new Dictionary<string, string>(t.Fields, StringComparer.OrdinalIgnoreCase)
                {
                    ["status"] = "Open",
                    ["movement"] = opType.ToString()
                };

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

                tripFields["loadStatus"] = loadStatus.ToString();
                if (tripPackages.HasValue) tripFields["loadedPackages"] = tripPackages.Value.ToString();
                if (tripWeight.HasValue) tripFields["loadedWeight"] = tripWeight.Value.ToString();
                if (tripCbm.HasValue) tripFields["loadedCbm"] = tripCbm.Value.ToString();

                var trip = new FtlOperationTrip(
                    tripGuid,
                    operation.Id,
                    ord++,
                    extId,
                    null,
                    null,
                    loadStatus,
                    tripPackages,
                    tripWeight,
                    tripCbm,
                    JsonSerializer.Serialize(tripFields));

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

                var tripFields = new Dictionary<string, string>(shared, StringComparer.OrdinalIgnoreCase)
                {
                    ["status"] = "Open",
                    ["movement"] = opType.ToString(),
                    ["loadStatus"] = FtlLoadStatus.Full.ToString()
                };

                if (packages > 0) tripFields["loadedPackages"] = packages.ToString();
                if (weight > 0) tripFields["loadedWeight"] = weight.ToString();
                if (cbm > 0) tripFields["loadedCbm"] = cbm.ToString();

                var trip = new FtlOperationTrip(
                    tripGuid,
                    operation.Id,
                    ord,
                    extId,
                    null,
                    null,
                    FtlLoadStatus.Full,
                    packages > 0 ? packages : null,
                    weight > 0 ? weight : null,
                    cbm > 0 ? cbm : null,
                    JsonSerializer.Serialize(tripFields));

                operation.Trips.Add(trip);
                tripRefs.Add(new CreateFtlTripRef(extId));
            }
        }

        await operationRepository.AddAsync(operation, ct);
        await unitOfWork.SaveChangesAsync(ct);

        var result = new CreateFtlOperationResult(
            opRef,
            operation.Version,
            today.ToString("yyyy-MM-dd"),
            timeNow.ToString("HH:mm"),
            tripRefs);

        return Result.Success(result);
    }
}

