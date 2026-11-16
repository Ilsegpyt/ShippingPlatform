using System.Text.Json;
using BuildingBlocks.Application;
using BuildingBlocks.Application.Abstractions;
using Customers.Contracts;
using MediatR;
using Operations.Application.Abstractions;
using Operations.Application.Operations.Ftl.Common;
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
        var year = now.Year;
        var today = DateOnly.FromDateTime(now);
        var timeNow = TimeOnly.FromDateTime(now);
        var userId = currentUser.UserId != Guid.Empty ? currentUser.UserId.ToString() : "u-1";
        var userName = await currentUser.GetUserNameAsync(ct);

        IReadOnlyList<CustomerOperationOption>? customersCache = null;

        // Group rows by Direction and Group (or MAWB)
        var groups = new Dictionary<string, List<ImportFtlTripRow>>();
        foreach (var t in command.Trips)
        {
            var grp = !string.IsNullOrWhiteSpace(t.Group)
                ? t.Group.Trim()
                : (t.Fields.GetValueOrDefault("bl") ?? t.Fields.GetValueOrDefault("booking") ?? Guid.NewGuid().ToString());

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

            var seq = await operationRepository.GetNextSequenceForYearAsync(year, ct);
            var opRef = $"OPS-{year}-{seq:D4}";

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

            var bl = firstFields.GetValueOrDefault("bl");
            var booking = firstFields.GetValueOrDefault("booking");
            var cert = firstFields.GetValueOrDefault("cert");
            var commodity = firstFields.GetValueOrDefault("commodity");
            var line = firstFields.GetValueOrDefault("line");
            var dest = firstFields.GetValueOrDefault("dest");
            var pod = firstFields.GetValueOrDefault("pod");

            int.TryParse(firstFields.GetValueOrDefault("packages"), out var packages);
            decimal.TryParse(firstFields.GetValueOrDefault("weight"), out var weight);
            decimal.TryParse(firstFields.GetValueOrDefault("cbm"), out var cbm);

            DateOnly? arrDate = DateOnly.TryParse(firstFields.GetValueOrDefault("arrDate"), out var ad) ? ad : null;
            TimeOnly? arrTime = TimeOnly.TryParse(firstFields.GetValueOrDefault("arrTime"), out var at) ? at : null;

            var operation = new FtlOperation(
                opRef,
                year,
                seq,
                opType,
                clientId,
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
                null,
                null,
                null,
                today,
                timeNow,
                userId,
                userName);

            operation.SetCreatedAudit(userId, userName, now);

            short ord = 0;
            foreach (var r in rows)
            {
                var fields = new Dictionary<string, string>(r.Fields, StringComparer.OrdinalIgnoreCase)
                {
                    ["status"] = "Open",
                    ["movement"] = opType.ToString()
                };
                FtlNormalizationHelper.NormalizeTripFields(fields);

                int.TryParse(fields.GetValueOrDefault("packages"), out var tripPackages);
                decimal.TryParse(fields.GetValueOrDefault("weight"), out var tripWeight);
                decimal.TryParse(fields.GetValueOrDefault("cbm"), out var tripCbm);

                var tripGuid = Guid.NewGuid();
                var trip = new FtlOperationTrip(
                    tripGuid,
                    operation.Id,
                    ord++,
                    tripGuid.ToString(),
                    null,
                    FtlLoadStatus.Full,
                    tripPackages > 0 ? tripPackages : null,
                    tripWeight > 0 ? tripWeight : null,
                    tripCbm > 0 ? tripCbm : null,
                    JsonSerializer.Serialize(fields));

                operation.Trips.Add(trip);
            }

            await operationRepository.AddAsync(operation, ct);
        }

        await unitOfWork.SaveChangesAsync(ct);

        var currentVersion = await operationRepository.GetCurrentWorkspaceVersionAsync(ct);
        return Result.Success(new ImportFtlTripsResult(currentVersion));
    }
}

