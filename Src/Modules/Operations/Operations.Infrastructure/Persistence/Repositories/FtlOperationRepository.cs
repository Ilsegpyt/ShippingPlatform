using System.Globalization;
using System.Linq.Expressions;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Operations.Application.Abstractions;
using Operations.Domain.Entities.Ftl;
using Operations.Domain.Enums;

namespace Operations.Infrastructure.Persistence.Repositories;

public sealed class FtlOperationRepository(OperationsDbContext dbContext) : IFtlOperationRepository
{
    public async Task<IReadOnlyList<FtlOperation>> GetAllActiveWithDetailsAsync(CancellationToken ct = default)
    {
        return await dbContext.FtlOperations
            .Include(x => x.Shipments)
            .Include(x => x.Trucks)
                .ThenInclude(t => t.Updates)
            .Include(x => x.Trips)
            .OrderByDescending(x => x.CreatedAtUtc)
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<FtlOperation>> GetWorkspaceWithDetailsAsync(
        string? operationType = null,
        bool onlyActive = false,
        CancellationToken ct = default)
    {
        var query = dbContext.FtlOperations
            .AsNoTracking()
            .AsSplitQuery();

        if (onlyActive)
        {
            query = query.Where(x => x.Status != OperationStatus.Delivered);
        }

        if (!string.IsNullOrWhiteSpace(operationType) &&
            Enum.TryParse<OperationType>(operationType, true, out var opType))
        {
            query = query.Where(x => x.OperationType == opType);
        }

        return await query
            .Include(x => x.Shipments)
            .Include(x => x.Trucks)
                .ThenInclude(t => t.Updates)
            .Include(x => x.Trips)
            .OrderByDescending(x => x.CreatedAtUtc)
            .ToListAsync(ct);
    }

    public async Task<FtlOperationsCountsDto> GetCountsAsync(CancellationToken ct = default)
    {
        var activeImport = await dbContext.FtlOperations.AsNoTracking().CountAsync(
            x => !x.IsDeleted && x.Status != OperationStatus.Delivered && x.OperationType == OperationType.Import,
            ct);

        var activeExport = await dbContext.FtlOperations.AsNoTracking().CountAsync(
            x => !x.IsDeleted && x.Status != OperationStatus.Delivered && x.OperationType == OperationType.Export,
            ct);

        var delivered = await dbContext.FtlOperations.AsNoTracking().CountAsync(
            x => !x.IsDeleted && x.Status == OperationStatus.Delivered,
            ct);

        var inactive = await dbContext.FtlOperations.IgnoreQueryFilters().AsNoTracking().CountAsync(
            x => x.IsDeleted,
            ct);

        return new FtlOperationsCountsDto(activeImport, activeExport, delivered, inactive);
    }

    public async Task<FtlFilterOptionsDto> GetFilterOptionsAsync(
        string? operationType = null,
        string? status = null,
        bool onlyActive = false,
        bool isDeleted = false,
        CancellationToken ct = default)
    {
        var query = isDeleted
            ? dbContext.FtlOperations.IgnoreQueryFilters().AsNoTracking().Where(x => x.IsDeleted)
            : dbContext.FtlOperations.AsNoTracking().Where(x => !x.IsDeleted);

        if (onlyActive)
        {
            query = query.Where(x => x.Status != OperationStatus.Delivered);
        }

        if (!string.IsNullOrWhiteSpace(operationType) &&
            Enum.TryParse<OperationType>(operationType, true, out var opType))
        {
            query = query.Where(x => x.OperationType == opType);
        }

        if (!string.IsNullOrWhiteSpace(status) &&
            Enum.TryParse<OperationStatus>(status, true, out var st))
        {
            query = query.Where(x => x.Status == st);
        }

        var clients = await query
            .SelectMany(x => x.Shipments)
            .Where(s => s.ClientName != null && s.ClientName.Trim() != "")
            .Select(s => s.ClientName!.Trim())
            .Distinct()
            .OrderBy(s => s)
            .ToListAsync(ct);

        var transporters = await query
            .SelectMany(x => x.Trucks)
            .Where(t => t.TransporterName != null && t.TransporterName.Trim() != "")
            .Select(t => t.TransporterName!.Trim())
            .Distinct()
            .OrderBy(s => s)
            .ToListAsync(ct);

        var terminals = await query
            .SelectMany(x => x.Shipments)
            .Where(s => s.Terminal != null && s.Terminal.Trim() != "")
            .Select(s => s.Terminal!.Trim())
            .Distinct()
            .OrderBy(s => s)
            .ToListAsync(ct);

        List<string> users;
        if (isDeleted)
        {
            users = await query
                .Select(x => (x.DeletedByName != null && x.DeletedByName != "") ? x.DeletedByName : x.CreatedByName)
                .Where(u => u != null && u.Trim() != "")
                .Select(u => u!.Trim())
                .Distinct()
                .OrderBy(s => s)
                .ToListAsync(ct);
        }
        else if (string.Equals(status, nameof(OperationStatus.Delivered), StringComparison.OrdinalIgnoreCase))
        {
            users = await query
                .Select(x => (x.DeliveredByName != null && x.DeliveredByName != "") ? x.DeliveredByName : x.CreatedByName)
                .Where(u => u != null && u.Trim() != "")
                .Select(u => u!.Trim())
                .Distinct()
                .OrderBy(s => s)
                .ToListAsync(ct);
        }
        else
        {
            users = await query
                .Select(x => x.CreatedByName)
                .Where(u => u != null && u.Trim() != "")
                .Select(u => u!.Trim())
                .Distinct()
                .OrderBy(s => s)
                .ToListAsync(ct);
        }

        return new FtlFilterOptionsDto(clients, transporters, terminals, users);
    }

    public async Task<PagedFtlOperationsResult> GetPagedAsync(
        FtlOperationQueryParams queryParams,
        CancellationToken ct = default)
    {
        var query = queryParams.IsDeleted
            ? dbContext.FtlOperations.IgnoreQueryFilters().AsNoTracking().Where(x => x.IsDeleted)
            : dbContext.FtlOperations.AsNoTracking().Where(x => !x.IsDeleted);

        if (queryParams.OnlyActive)
        {
            query = query.Where(x => x.Status != OperationStatus.Delivered);
        }

        if (!string.IsNullOrWhiteSpace(queryParams.OperationType) &&
            Enum.TryParse<OperationType>(queryParams.OperationType, true, out var opType))
        {
            query = query.Where(x => x.OperationType == opType);
        }

        if (!string.IsNullOrWhiteSpace(queryParams.Status) &&
            Enum.TryParse<OperationStatus>(queryParams.Status, true, out var st))
        {
            query = query.Where(x => x.Status == st);
        }

        if (!string.IsNullOrWhiteSpace(queryParams.Search))
        {
            query = query.Where(BuildQuickSearchPredicate(queryParams.Search.Trim()));
        }

        if (!string.IsNullOrWhiteSpace(queryParams.RulesJson))
        {
            var rulesPredicate = BuildRulesPredicate(queryParams.RulesJson);
            if (rulesPredicate is not null)
            {
                query = query.Where(rulesPredicate);
            }
        }

        if (!string.IsNullOrWhiteSpace(queryParams.Client))
        {
            var cTerm = queryParams.Client.Trim();
            query = query.Where(x => x.Shipments.Any(s => s.ClientName != null && s.ClientName.Trim() == cTerm));
        }

        if (!string.IsNullOrWhiteSpace(queryParams.Transporter))
        {
            var trTerm = queryParams.Transporter.Trim();
            query = query.Where(x => x.Trucks.Any(t => t.TransporterName != null && t.TransporterName.Trim() == trTerm));
        }

        if (!string.IsNullOrWhiteSpace(queryParams.Terminal))
        {
            var termTerm = queryParams.Terminal.Trim();
            query = query.Where(x => x.Shipments.Any(s => s.Terminal != null && s.Terminal.Trim() == termTerm));
        }

        if (!string.IsNullOrWhiteSpace(queryParams.CreatedBy))
        {
            var cbTerm = queryParams.CreatedBy.Trim();
            if (queryParams.IsDeleted)
            {
                query = query.Where(x =>
                    ((x.DeletedByName != null && x.DeletedByName != "") ? x.DeletedByName : x.CreatedByName).Trim() == cbTerm);
            }
            else if (string.Equals(queryParams.Status, nameof(OperationStatus.Delivered), StringComparison.OrdinalIgnoreCase))
            {
                query = query.Where(x =>
                    ((x.DeliveredByName != null && x.DeliveredByName != "") ? x.DeliveredByName : x.CreatedByName).Trim() == cbTerm);
            }
            else
            {
                query = query.Where(x => x.CreatedByName.Trim() == cbTerm);
            }
        }

        DateTime? dateFromUtc = null;
        DateTime? dateToUtc = null;
        if (!string.IsNullOrWhiteSpace(queryParams.DateFrom) && DateTime.TryParse(queryParams.DateFrom, out var dFrom))
        {
            dateFromUtc = DateTime.SpecifyKind(dFrom.Date, DateTimeKind.Utc);
        }
        if (!string.IsNullOrWhiteSpace(queryParams.DateTo) && DateTime.TryParse(queryParams.DateTo, out var dTo))
        {
            dateToUtc = DateTime.SpecifyKind(dTo.Date.AddDays(1).AddTicks(-1), DateTimeKind.Utc);
        }

        if (dateFromUtc.HasValue || dateToUtc.HasValue)
        {
            var dType = queryParams.DateType?.Trim().ToLowerInvariant();
            if (dType == "delivered")
            {
                query = query.Where(x =>
                    (!dateFromUtc.HasValue || ((x.DeliveredAtUtc ?? x.CreatedAtUtc) >= dateFromUtc.Value)) &&
                    (!dateToUtc.HasValue || ((x.DeliveredAtUtc ?? x.CreatedAtUtc) <= dateToUtc.Value)));
            }
            else if (dType == "inactive")
            {
                query = query.Where(x =>
                    (!dateFromUtc.HasValue || ((x.DeletedAtUtc ?? x.CreatedAtUtc) >= dateFromUtc.Value)) &&
                    (!dateToUtc.HasValue || ((x.DeletedAtUtc ?? x.CreatedAtUtc) <= dateToUtc.Value)));
            }
            else
            {
                query = query.Where(x =>
                    (!dateFromUtc.HasValue || x.CreatedAtUtc >= dateFromUtc.Value) &&
                    (!dateToUtc.HasValue || x.CreatedAtUtc <= dateToUtc.Value));
            }
        }

        var totalCount = await query.CountAsync(ct);
        var totalTrucks = await query.SelectMany(x => x.Trucks).CountAsync(ct);

        var page = Math.Max(1, queryParams.Page);
        var pageSize = Math.Clamp(queryParams.PageSize, 1, 100);
        var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

        var isAsc = string.Equals(queryParams.SortDirection, "asc", StringComparison.OrdinalIgnoreCase);
        var sortBy = queryParams.SortBy?.Trim().ToLowerInvariant();

        query = sortBy switch
        {
            "__tripno" => isAsc
                ? query.OrderByDescending(x => x.CreatedAtUtc)
                : query.OrderBy(x => x.CreatedAtUtc),
            "operationnumber" or "ref" or "__ref" => isAsc
                ? query.OrderBy(x => x.Id)
                : query.OrderByDescending(x => x.Id),
            "shipno" => isAsc
                ? query.OrderBy(x => x.Shipments.Select(s => s.ShipmentNumber).FirstOrDefault())
                : query.OrderByDescending(x => x.Shipments.Select(s => s.ShipmentNumber).FirstOrDefault()),
            "vol" => isAsc
                ? query.OrderBy(x => x.NumberOfShipments)
                : query.OrderByDescending(x => x.NumberOfShipments),
            "oper" => isAsc
                ? query.OrderBy(x => x.CreatedByName)
                : query.OrderByDescending(x => x.CreatedByName),
            "clrtype" => isAsc
                ? query.OrderBy(x => x.Shipments.Select(s => s.ClearanceType).FirstOrDefault())
                : query.OrderByDescending(x => x.Shipments.Select(s => s.ClearanceType).FirstOrDefault()),
            "cert" => isAsc
                ? query.OrderBy(x => x.Shipments.Select(s => s.CertificateNumber).FirstOrDefault())
                : query.OrderByDescending(x => x.Shipments.Select(s => s.CertificateNumber).FirstOrDefault()),
            "customs" => isAsc
                ? query.OrderBy(x => x.Shipments.Select(s => s.CustomsArea).FirstOrDefault())
                : query.OrderByDescending(x => x.Shipments.Select(s => s.CustomsArea).FirstOrDefault()),
            "client" or "clientname" => isAsc
                ? query.OrderBy(x => x.Shipments.Select(s => s.ClientName).FirstOrDefault())
                : query.OrderByDescending(x => x.Shipments.Select(s => s.ClientName).FirstOrDefault()),
            "mawb" or "bl" or "booking" => isAsc
                ? query.OrderBy(x => x.Shipments.Select(s => s.MAWB).FirstOrDefault())
                : query.OrderByDescending(x => x.Shipments.Select(s => s.MAWB).FirstOrDefault()),
            "hawb" => isAsc
                ? query.OrderBy(x => x.Shipments.Select(s => s.HAWB).FirstOrDefault())
                : query.OrderByDescending(x => x.Shipments.Select(s => s.HAWB).FirstOrDefault()),
            "line" => isAsc
                ? query.OrderBy(x => x.Shipments.Select(s => s.Terminal).FirstOrDefault())
                : query.OrderByDescending(x => x.Shipments.Select(s => s.Terminal).FirstOrDefault()),
            "dest" => isAsc
                ? query.OrderBy(x => x.Shipments.Select(s => s.Destination).FirstOrDefault())
                : query.OrderByDescending(x => x.Shipments.Select(s => s.Destination).FirstOrDefault()),
            "fulladdress" => isAsc
                ? query.OrderBy(x => x.Shipments.Select(s => s.FullAddress).FirstOrDefault())
                : query.OrderByDescending(x => x.Shipments.Select(s => s.FullAddress).FirstOrDefault()),
            "aod" => isAsc
                ? query.OrderBy(x => x.Shipments.Select(s => s.AOD).FirstOrDefault())
                : query.OrderByDescending(x => x.Shipments.Select(s => s.AOD).FirstOrDefault()),
            "trucker" => isAsc
                ? query.OrderBy(x => x.Trucks.Select(t => t.TransporterName).FirstOrDefault())
                : query.OrderByDescending(x => x.Trucks.Select(t => t.TransporterName).FirstOrDefault()),
            "car" or "cntr" => isAsc
                ? query.OrderBy(x => x.Trucks.Select(t => t.TruckPlateNumber).FirstOrDefault())
                : query.OrderByDescending(x => x.Trucks.Select(t => t.TruckPlateNumber).FirstOrDefault()),
            "driver" => isAsc
                ? query.OrderBy(x => x.Trucks.Select(t => t.DriverName).FirstOrDefault())
                : query.OrderByDescending(x => x.Trucks.Select(t => t.DriverName).FirstOrDefault()),
            "mob" => isAsc
                ? query.OrderBy(x => x.Trucks.Select(t => t.DriverMobile).FirstOrDefault())
                : query.OrderByDescending(x => x.Trucks.Select(t => t.DriverMobile).FirstOrDefault()),
            "reqdate" => isAsc
                ? query.OrderBy(x => x.Trucks.Select(t => t.RequiredArrivalDate).FirstOrDefault())
                : query.OrderByDescending(x => x.Trucks.Select(t => t.RequiredArrivalDate).FirstOrDefault()),
            "reqtime" => isAsc
                ? query.OrderBy(x => x.Trucks.Select(t => t.RequiredArrivalTime).FirstOrDefault())
                : query.OrderByDescending(x => x.Trucks.Select(t => t.RequiredArrivalTime).FirstOrDefault()),
            "arrdate" => isAsc
                ? query.OrderBy(x => x.Trucks.Select(t => t.ArrivalDate).FirstOrDefault())
                : query.OrderByDescending(x => x.Trucks.Select(t => t.ArrivalDate).FirstOrDefault()),
            "arrtime" => isAsc
                ? query.OrderBy(x => x.Trucks.Select(t => t.ArrivalTime).FirstOrDefault())
                : query.OrderByDescending(x => x.Trucks.Select(t => t.ArrivalTime).FirstOrDefault()),
            "gateout" => isAsc
                ? query.OrderBy(x => x.Trucks.Select(t => t.GateOutDate).FirstOrDefault())
                : query.OrderByDescending(x => x.Trucks.Select(t => t.GateOutDate).FirstOrDefault()),
            "gateoutt" => isAsc
                ? query.OrderBy(x => x.Trucks.Select(t => t.GateOutTime).FirstOrDefault())
                : query.OrderByDescending(x => x.Trucks.Select(t => t.GateOutTime).FirstOrDefault()),
            "invrcv" => isAsc
                ? query.OrderBy(x => x.Shipments.Select(s => s.InvoicesReceivedDate).FirstOrDefault())
                : query.OrderByDescending(x => x.Shipments.Select(s => s.InvoicesReceivedDate).FirstOrDefault()),
            "missdoc" => isAsc
                ? query.OrderBy(x => x.Shipments.Select(s => s.MissingDocs).FirstOrDefault())
                : query.OrderByDescending(x => x.Shipments.Select(s => s.MissingDocs).FirstOrDefault()),
            "missrcv" => isAsc
                ? query.OrderBy(x => x.Shipments.Select(s => s.MissingDocsReceivedDate).FirstOrDefault())
                : query.OrderByDescending(x => x.Shipments.Select(s => s.MissingDocsReceivedDate).FirstOrDefault()),
            "loadedpackages" => isAsc
                ? query.OrderBy(x => x.Trips.Select(tr => tr.LoadedPackages).FirstOrDefault())
                : query.OrderByDescending(x => x.Trips.Select(tr => tr.LoadedPackages).FirstOrDefault()),
            "loadstatus" => isAsc
                ? query.OrderBy(x => x.Trips.Select(tr => tr.LoadStatus).FirstOrDefault())
                : query.OrderByDescending(x => x.Trips.Select(tr => tr.LoadStatus).FirstOrDefault()),
            "status" => isAsc
                ? query.OrderBy(x => x.Status)
                : query.OrderByDescending(x => x.Status),
            "deliveredat" => isAsc
                ? query.OrderBy(x => x.DeliveredAtUtc ?? x.CreatedAtUtc)
                : query.OrderByDescending(x => x.DeliveredAtUtc ?? x.CreatedAtUtc),
            "createdat" or "date" or "__created" or "day" => isAsc
                ? query.OrderBy(x => x.CreatedAtUtc)
                : query.OrderByDescending(x => x.CreatedAtUtc),
            _ => isAsc
                ? query.OrderBy(x => queryParams.IsDeleted ? (x.DeletedAtUtc ?? x.CreatedAtUtc) : (x.Status == OperationStatus.Delivered ? (x.DeliveredAtUtc ?? x.CreatedAtUtc) : x.CreatedAtUtc))
                : query.OrderByDescending(x => queryParams.IsDeleted ? (x.DeletedAtUtc ?? x.CreatedAtUtc) : (x.Status == OperationStatus.Delivered ? (x.DeliveredAtUtc ?? x.CreatedAtUtc) : x.CreatedAtUtc))
        };

        var items = await query
            .AsSplitQuery()
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Include(x => x.Shipments)
            .Include(x => x.Trucks)
                .ThenInclude(t => t.Updates)
            .Include(x => x.Trips)
            .Include(x => x.StatusAudits)
            .ToListAsync(ct);

        return new PagedFtlOperationsResult(items, totalCount, totalTrucks, page, pageSize, totalPages);
    }

    public async Task<FtlOperation?> GetByIdForUpdateAsync(Guid id, CancellationToken ct = default)
    {
        return await dbContext.FtlOperations
            .Include(x => x.Shipments)
            .Include(x => x.Trucks)
                .ThenInclude(t => t.Updates)
            .Include(x => x.Trips)
            .Include(x => x.StatusAudits)
            .FirstOrDefaultAsync(x => x.Id == id, ct);
    }

    public async Task<FtlOperation?> GetByRefAsync(string reference, bool includeDetails = true, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(reference)) return null;

        var clean = reference.Trim();
        if (clean.StartsWith("OPS-", StringComparison.OrdinalIgnoreCase))
        {
            clean = clean[4..];
        }

        var query = dbContext.FtlOperations.AsQueryable();

        if (includeDetails)
        {
            query = query
                .Include(x => x.Shipments)
                .Include(x => x.Trucks)
                    .ThenInclude(t => t.Updates)
                .Include(x => x.Trips)
                .Include(x => x.StatusAudits);
        }

        if (Guid.TryParse(clean, out var parsedGuid))
        {
            var byId = await query.FirstOrDefaultAsync(x => x.Id == parsedGuid, ct);
            if (byId is not null) return byId;
        }

        var lowerPrefix = clean.ToLowerInvariant();
        var match = await query.FirstOrDefaultAsync(x => x.Id.ToString().StartsWith(lowerPrefix), ct);
        if (match is not null) return match;

        var upperPrefix = clean.ToUpperInvariant();
        return await query.FirstOrDefaultAsync(x => x.Id.ToString().StartsWith(upperPrefix), ct);
    }

    public async Task<FtlOperation?> GetByIdAsync(Guid id, bool includeDetails = true, CancellationToken ct = default)
    {
        var query = dbContext.FtlOperations.AsQueryable();

        if (includeDetails)
        {
            query = query
                .Include(x => x.Shipments)
                .Include(x => x.Trucks)
                    .ThenInclude(t => t.Updates)
                .Include(x => x.Trips)
                .Include(x => x.StatusAudits);
        }

        return await query.FirstOrDefaultAsync(x => x.Id == id, ct);
    }

    public async Task AddAsync(FtlOperation operation, CancellationToken ct = default)
    {
        await dbContext.FtlOperations.AddAsync(operation, ct);
    }

    public async Task AddShipmentAsync(FtlShipment shipment, CancellationToken ct = default)
    {
        await dbContext.FtlShipments.AddAsync(shipment, ct);
    }

    public async Task AddTruckAsync(FtlTruck truck, CancellationToken ct = default)
    {
        await dbContext.FtlTrucks.AddAsync(truck, ct);
    }

    public async Task AddTruckUpdateAsync(FtlTruckUpdate update, CancellationToken ct = default)
    {
        await dbContext.FtlTruckUpdates.AddAsync(update, ct);
    }

    public async Task AddTripAsync(FtlOperationTrip trip, CancellationToken ct = default)
    {
        await dbContext.FtlOperationTrips.AddAsync(trip, ct);
    }

    public Task DeleteShipmentsAsync(IEnumerable<FtlShipment> shipments, CancellationToken ct = default)
    {
        dbContext.FtlShipments.RemoveRange(shipments);
        return Task.CompletedTask;
    }

    public Task DeleteTripsAsync(IEnumerable<FtlOperationTrip> trips, CancellationToken ct = default)
    {
        dbContext.FtlOperationTrips.RemoveRange(trips);
        return Task.CompletedTask;
    }

    public Task DeleteTrucksAsync(IEnumerable<FtlTruck> trucks, CancellationToken ct = default)
    {
        dbContext.FtlTrucks.RemoveRange(trucks);
        return Task.CompletedTask;
    }

    public async Task<bool> ExistsByRefAsync(string reference, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(reference)) return false;
        var clean = reference.Trim();
        if (clean.StartsWith("OPS-", StringComparison.OrdinalIgnoreCase)) clean = clean[4..];
        if (Guid.TryParse(clean, out var parsedGuid))
        {
            return await dbContext.FtlOperations.AnyAsync(x => x.Id == parsedGuid, ct);
        }
        var prefix = clean.ToLowerInvariant();
        return await dbContext.FtlOperations.AnyAsync(x => x.Id.ToString().StartsWith(prefix), ct);
    }

    public async Task<bool> ShipmentNumberExistsAsync(string shipNo, Guid? excludeOperationId = null, CancellationToken ct = default)
    {
        var norm = Operations.Domain.Common.IdentifierNormalizer.Normalize(shipNo);
        if (string.IsNullOrWhiteSpace(norm)) return false;
        return await dbContext.FtlShipments
            .AnyAsync(s => !s.Operation.IsDeleted && s.ShipmentNumber == norm && (!excludeOperationId.HasValue || s.FtlOperationId != excludeOperationId.Value), ct);
    }

    public async Task<bool> HawbExistsAsync(string hawb, Guid? excludeOperationId = null, CancellationToken ct = default)
    {
        var norm = Operations.Domain.Common.IdentifierNormalizer.Normalize(hawb);
        if (string.IsNullOrWhiteSpace(norm)) return false;
        return await dbContext.FtlShipments
            .AnyAsync(s => !s.Operation.IsDeleted && s.HAWB == norm && (!excludeOperationId.HasValue || s.FtlOperationId != excludeOperationId.Value), ct);
    }

    public async Task<bool> MawbHawbPairExistsAsync(string mawb, string hawb, Guid? excludeOperationId = null, CancellationToken ct = default)
    {
        var mNorm = Operations.Domain.Common.IdentifierNormalizer.Normalize(mawb);
        var hNorm = Operations.Domain.Common.IdentifierNormalizer.Normalize(hawb);
        if (string.IsNullOrWhiteSpace(mNorm) || string.IsNullOrWhiteSpace(hNorm)) return false;
        return await dbContext.FtlShipments
            .AnyAsync(s => !s.Operation.IsDeleted && s.MAWB == mNorm && s.HAWB == hNorm && (!excludeOperationId.HasValue || s.FtlOperationId != excludeOperationId.Value), ct);
    }

    public async Task<bool> CertShipmentPairExistsAsync(string cert, string shipNo, Guid? excludeOperationId = null, CancellationToken ct = default)
    {
        var cNorm = Operations.Domain.Common.IdentifierNormalizer.Normalize(cert);
        var sNorm = Operations.Domain.Common.IdentifierNormalizer.Normalize(shipNo);
        if (string.IsNullOrWhiteSpace(cNorm) || string.IsNullOrWhiteSpace(sNorm)) return false;
        return await dbContext.FtlShipments
            .AnyAsync(s => !s.Operation.IsDeleted
                && s.CertificateNumber == cNorm
                && s.ShipmentNumber == sNorm
                && (!excludeOperationId.HasValue || s.FtlOperationId != excludeOperationId.Value), ct);
    }

    private static Expression<Func<FtlOperation, bool>> BuildQuickSearchPredicate(string term)
    {
        var cleanRef = term.StartsWith("OPS-", StringComparison.OrdinalIgnoreCase)
            ? term[4..]
            : term;

        return x =>
            EF.Functions.Like(x.Id.ToString(), $"%{cleanRef}%") ||
            (x.CreatedByName != null && EF.Functions.Like(x.CreatedByName, $"%{term}%")) ||
            x.Shipments.Any(s =>
                (s.ShipmentNumber != null && EF.Functions.Like(s.ShipmentNumber, $"%{term}%")) ||
                (s.HAWB != null && EF.Functions.Like(s.HAWB, $"%{term}%")) ||
                (s.MAWB != null && EF.Functions.Like(s.MAWB, $"%{term}%")) ||
                (s.ClientName != null && EF.Functions.Like(s.ClientName, $"%{term}%")) ||
                (s.CertificateNumber != null && EF.Functions.Like(s.CertificateNumber, $"%{term}%")) ||
                (s.CustomsArea != null && EF.Functions.Like(s.CustomsArea, $"%{term}%")) ||
                (s.Terminal != null && EF.Functions.Like(s.Terminal, $"%{term}%")) ||
                (s.Destination != null && EF.Functions.Like(s.Destination, $"%{term}%")) ||
                (s.AOD != null && EF.Functions.Like(s.AOD, $"%{term}%"))
            ) ||
            x.Trucks.Any(t =>
                (t.TruckPlateNumber != null && EF.Functions.Like(t.TruckPlateNumber, $"%{term}%")) ||
                (t.DriverName != null && EF.Functions.Like(t.DriverName, $"%{term}%")) ||
                (t.DriverMobile != null && EF.Functions.Like(t.DriverMobile, $"%{term}%")) ||
                (t.TransporterName != null && EF.Functions.Like(t.TransporterName, $"%{term}%")) ||
                t.Updates.Any(u => u.Note != null && EF.Functions.Like(u.Note, $"%{term}%"))
            );
    }

    private static Expression<Func<FtlOperation, bool>>? BuildRulesPredicate(string rulesJson)
    {
        List<FtlSearchRuleItem>? rules;
        try
        {
            rules = JsonSerializer.Deserialize<List<FtlSearchRuleItem>>(rulesJson);
        }
        catch
        {
            return null;
        }

        if (rules is null || rules.Count == 0) return null;

        var activeRules = rules
            .Where(r => !string.IsNullOrWhiteSpace(r.V))
            .ToList();

        if (activeRules.Count == 0) return null;

        var param = Expression.Parameter(typeof(FtlOperation), "x");
        Expression? combined = null;

        foreach (var rule in activeRules)
        {
            var pred = BuildSingleRulePredicate(rule.K ?? "__all", rule.V!.Trim());
            if (pred is null) continue;

            var body = ParameterReplacer.Replace(pred.Body, pred.Parameters[0], param);
            if (combined is null)
            {
                combined = body;
            }
            else
            {
                var join = (rule.Join ?? "AND").Trim().ToUpperInvariant();
                combined = join switch
                {
                    "OR" => Expression.OrElse(combined, body),
                    "NOT" => Expression.AndAlso(combined, Expression.Not(body)),
                    _ => Expression.AndAlso(combined, body)
                };
            }
        }

        return combined is null ? null : Expression.Lambda<Func<FtlOperation, bool>>(combined, param);
    }

    private static Expression<Func<FtlOperation, bool>>? BuildSingleRulePredicate(string key, string term)
    {
        var k = key.Trim().ToLowerInvariant();
        switch (k)
        {
            case "__all":
                return BuildQuickSearchPredicate(term);

            case "__ref":
            {
                var cleanRef = term.StartsWith("OPS-", StringComparison.OrdinalIgnoreCase) ? term[4..] : term;
                return x => EF.Functions.Like(x.Id.ToString(), $"%{cleanRef}%");
            }

            case "__created":
            case "day":
                if (TryParseFlexibleDate(term, out var createdDate))
                {
                    var dayStart = DateTime.SpecifyKind(createdDate.ToDateTime(TimeOnly.MinValue), DateTimeKind.Utc);
                    var dayEnd = dayStart.AddDays(1);
                    return x => x.CreatedAtUtc >= dayStart && x.CreatedAtUtc < dayEnd;
                }
                return null;

            case "shipno":
                return x => x.Shipments.Any(s => s.ShipmentNumber != null && EF.Functions.Like(s.ShipmentNumber, $"%{term}%"));

            case "vol":
                if (int.TryParse(term, out var volVal))
                    return x => x.NumberOfShipments == volVal;
                return _ => false;

            case "oper":
                return x => x.CreatedByName != null && EF.Functions.Like(x.CreatedByName, $"%{term}%");

            case "clrtype":
                if (term.Contains("bosla", StringComparison.OrdinalIgnoreCase))
                    return x => x.Shipments.Any(s => s.ClearanceType == ClearanceType.Bosla);
                if (term.Contains("cert", StringComparison.OrdinalIgnoreCase))
                    return x => x.Shipments.Any(s => s.ClearanceType == ClearanceType.Cert);
                return _ => false;

            case "cert":
                return x => x.Shipments.Any(s => s.CertificateNumber != null && EF.Functions.Like(s.CertificateNumber, $"%{term}%"));

            case "customs":
                return x => x.Shipments.Any(s => s.CustomsArea != null && EF.Functions.Like(s.CustomsArea, $"%{term}%"));

            case "client":
                return x => x.Shipments.Any(s => s.ClientName != null && EF.Functions.Like(s.ClientName, $"%{term}%"));

            case "mawb":
            case "bl":
            case "booking":
                return x => x.Shipments.Any(s => s.MAWB != null && EF.Functions.Like(s.MAWB, $"%{term}%"));

            case "hawb":
                return x => x.Shipments.Any(s => s.HAWB != null && EF.Functions.Like(s.HAWB, $"%{term}%"));

            case "line":
                return x => x.Shipments.Any(s => s.Terminal != null && EF.Functions.Like(s.Terminal, $"%{term}%"));

            case "dest":
                return x => x.Shipments.Any(s => s.Destination != null && EF.Functions.Like(s.Destination, $"%{term}%"));

            case "fulladdress":
                return x => x.Shipments.Any(s => s.FullAddress != null && EF.Functions.Like(s.FullAddress, $"%{term}%"));

            case "aod":
                return x => x.Shipments.Any(s => s.AOD != null && EF.Functions.Like(s.AOD, $"%{term}%"));

            case "trucker":
                return x => x.Trucks.Any(t => t.TransporterName != null && EF.Functions.Like(t.TransporterName, $"%{term}%"));

            case "car":
            case "cntr":
                return x => x.Trucks.Any(t => t.TruckPlateNumber != null && EF.Functions.Like(t.TruckPlateNumber, $"%{term}%"));

            case "driver":
                return x => x.Trucks.Any(t => t.DriverName != null && EF.Functions.Like(t.DriverName, $"%{term}%"));

            case "mob":
                return x => x.Trucks.Any(t => t.DriverMobile != null && EF.Functions.Like(t.DriverMobile, $"%{term}%"));

            case "missdoc":
                return x => x.Shipments.Any(s => s.MissingDocs != null && EF.Functions.Like(s.MissingDocs, $"%{term}%"));

            case "loadedpackages":
                if (int.TryParse(term, out var lpVal))
                    return x => x.Trips.Any(tr => tr.LoadedPackages == lpVal);
                return _ => false;

            case "loadstatus":
                if (Enum.TryParse<FtlLoadStatus>(term, true, out var lsVal))
                    return x => x.Trips.Any(tr => tr.LoadStatus == lsVal);
                return _ => false;

            case "status":
                if (Enum.TryParse<OperationStatus>(term, true, out var stVal))
                    return x => x.Status == stVal;
                return _ => false;

            case "updates":
            case "upddate":
                return x => x.Trucks.Any(t => t.Updates.Any(u => u.Note != null && EF.Functions.Like(u.Note, $"%{term}%")));

            case "reqdate":
                return TryParseFlexibleDate(term, out var reqD)
                    ? x => x.Trucks.Any(t => t.RequiredArrivalDate == reqD)
                    : null;

            case "arrdate":
                return TryParseFlexibleDate(term, out var arrD)
                    ? x => x.Trucks.Any(t => t.ArrivalDate == arrD)
                    : null;

            case "gateout":
                return TryParseFlexibleDate(term, out var goD)
                    ? x => x.Trucks.Any(t => t.GateOutDate == goD)
                    : null;

            case "invrcv":
                return TryParseFlexibleDate(term, out var invD)
                    ? x => x.Shipments.Any(s => s.InvoicesReceivedDate == invD)
                    : null;

            case "missrcv":
                return TryParseFlexibleDate(term, out var missD)
                    ? x => x.Shipments.Any(s => s.MissingDocsReceivedDate == missD)
                    : null;

            case "reqtime":
                return TryParseFlexibleTime(term, out var reqT)
                    ? x => x.Trucks.Any(t => t.RequiredArrivalTime.HasValue && t.RequiredArrivalTime.Value.Hour == reqT.Hour && t.RequiredArrivalTime.Value.Minute == reqT.Minute)
                    : null;

            case "arrtime":
                return TryParseFlexibleTime(term, out var arrT)
                    ? x => x.Trucks.Any(t => t.ArrivalTime.HasValue && t.ArrivalTime.Value.Hour == arrT.Hour && t.ArrivalTime.Value.Minute == arrT.Minute)
                    : null;

            case "gateoutt":
                return TryParseFlexibleTime(term, out var goT)
                    ? x => x.Trucks.Any(t => t.GateOutTime.HasValue && t.GateOutTime.Value.Hour == goT.Hour && t.GateOutTime.Value.Minute == goT.Minute)
                    : null;

            default:
                return null;
        }
    }

    private static bool TryParseFlexibleDate(string input, out DateOnly date)
    {
        var s = input.Trim();
        string[] formats = ["yyyy-MM-dd", "dd-MMM-yyyy", "d-MMM-yyyy", "dd/MM/yyyy", "d/M/yyyy", "dd-MM-yyyy", "d-M-yyyy"];
        if (DateOnly.TryParseExact(s, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out date))
            return true;

        // Support short d/M or d-M using current UTC year
        string[] shortFormats = ["d/M", "dd/MM", "d-M", "dd-MM"];
        if (DateOnly.TryParseExact(s, shortFormats, CultureInfo.InvariantCulture, DateTimeStyles.None, out var partial))
        {
            date = new DateOnly(DateTime.UtcNow.Year, partial.Month, partial.Day);
            return true;
        }

        return DateOnly.TryParse(s, CultureInfo.InvariantCulture, DateTimeStyles.None, out date);
    }

    private static bool TryParseFlexibleTime(string input, out TimeOnly time)
    {
        var s = input.Trim().ToLowerInvariant();
        if (int.TryParse(s, out var hrOnly) && hrOnly >= 0 && hrOnly <= 23)
        {
            time = new TimeOnly(hrOnly, 0);
            return true;
        }

        if (s.EndsWith('p') || s.EndsWith('a'))
            s += "m";

        string[] formats = ["H:mm", "HH:mm", "h:mmtt", "h:mm tt", "htt", "h tt"];
        if (TimeOnly.TryParseExact(s, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out time))
            return true;

        return TimeOnly.TryParse(s, CultureInfo.InvariantCulture, DateTimeStyles.None, out time);
    }

    private sealed class FtlSearchRuleItem
    {
        [JsonPropertyName("k")]
        public string? K { get; set; }

        [JsonPropertyName("v")]
        public string? V { get; set; }

        [JsonPropertyName("join")]
        public string? Join { get; set; }
    }

    private sealed class ParameterReplacer(ParameterExpression source, ParameterExpression target) : ExpressionVisitor
    {
        public static Expression Replace(Expression body, ParameterExpression source, ParameterExpression target)
            => new ParameterReplacer(source, target).Visit(body);

        protected override Expression VisitParameter(ParameterExpression node)
            => node == source ? target : base.VisitParameter(node);
    }
}

