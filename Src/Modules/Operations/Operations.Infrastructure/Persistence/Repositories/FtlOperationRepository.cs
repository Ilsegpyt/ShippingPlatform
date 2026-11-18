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
            var term = queryParams.Search.Trim();
            var cleanRef = term.StartsWith("OPS-", StringComparison.OrdinalIgnoreCase)
                ? term[4..]
                : term;

            query = query.Where(x =>
                EF.Functions.Like(x.Id.ToString(), $"%{cleanRef}%") ||
                x.Shipments.Any(s =>
                    (s.ShipmentNumber != null && EF.Functions.Like(s.ShipmentNumber, $"%{term}%")) ||
                    (s.HAWB != null && EF.Functions.Like(s.HAWB, $"%{term}%")) ||
                    (s.MAWB != null && EF.Functions.Like(s.MAWB, $"%{term}%")) ||
                    (s.ClientName != null && EF.Functions.Like(s.ClientName, $"%{term}%")) ||
                    (s.CertificateNumber != null && EF.Functions.Like(s.CertificateNumber, $"%{term}%"))
                ) ||
                x.Trucks.Any(t =>
                    (t.TruckPlateNumber != null && EF.Functions.Like(t.TruckPlateNumber, $"%{term}%")) ||
                    (t.DriverName != null && EF.Functions.Like(t.DriverName, $"%{term}%")) ||
                    (t.TransporterName != null && EF.Functions.Like(t.TransporterName, $"%{term}%"))
                )
            );
        }

        if (!string.IsNullOrWhiteSpace(queryParams.Client))
        {
            var cTerm = queryParams.Client.Trim();
            query = query.Where(x => x.Shipments.Any(s => s.ClientName != null && EF.Functions.Like(s.ClientName, $"%{cTerm}%")));
        }

        if (!string.IsNullOrWhiteSpace(queryParams.Transporter))
        {
            var trTerm = queryParams.Transporter.Trim();
            query = query.Where(x => x.Trucks.Any(t => t.TransporterName != null && EF.Functions.Like(t.TransporterName, $"%{trTerm}%")));
        }

        if (!string.IsNullOrWhiteSpace(queryParams.Terminal))
        {
            var termTerm = queryParams.Terminal.Trim();
            query = query.Where(x => x.Shipments.Any(s => s.Terminal != null && EF.Functions.Like(s.Terminal, $"%{termTerm}%")));
        }

        if (!string.IsNullOrWhiteSpace(queryParams.CreatedBy))
        {
            var cbTerm = queryParams.CreatedBy.Trim();
            query = query.Where(x =>
                (x.DeliveredByName != null && EF.Functions.Like(x.DeliveredByName, $"%{cbTerm}%")) ||
                (x.CreatedByName != null && EF.Functions.Like(x.CreatedByName, $"%{cbTerm}%")) ||
                (x.DeletedByName != null && EF.Functions.Like(x.DeletedByName, $"%{cbTerm}%")));
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
            "operationnumber" or "ref" or "__ref" => isAsc
                ? query.OrderBy(x => x.Id)
                : query.OrderByDescending(x => x.Id),
            "client" or "clientname" => isAsc
                ? query.OrderBy(x => x.Shipments.Select(s => s.ClientName).FirstOrDefault())
                : query.OrderByDescending(x => x.Shipments.Select(s => s.ClientName).FirstOrDefault()),
            "mawb" => isAsc
                ? query.OrderBy(x => x.Shipments.Select(s => s.MAWB).FirstOrDefault())
                : query.OrderByDescending(x => x.Shipments.Select(s => s.MAWB).FirstOrDefault()),
            "status" => isAsc
                ? query.OrderBy(x => x.Status)
                : query.OrderByDescending(x => x.Status),
            "deliveredat" => isAsc
                ? query.OrderBy(x => x.DeliveredAtUtc ?? x.CreatedAtUtc)
                : query.OrderByDescending(x => x.DeliveredAtUtc ?? x.CreatedAtUtc),
            "createdat" or "date" or "__created" => isAsc
                ? query.OrderBy(x => x.CreatedAtUtc)
                : query.OrderByDescending(x => x.CreatedAtUtc),
            _ => isAsc
                ? query.OrderBy(x => queryParams.IsDeleted ? (x.DeletedAtUtc ?? x.CreatedAtUtc) : (x.Status == OperationStatus.Delivered ? (x.DeliveredAtUtc ?? x.CreatedAtUtc) : x.CreatedAtUtc))
                : query.OrderByDescending(x => queryParams.IsDeleted ? (x.DeletedAtUtc ?? x.CreatedAtUtc) : (x.Status == OperationStatus.Delivered ? (x.DeliveredAtUtc ?? x.CreatedAtUtc) : x.CreatedAtUtc))
        };

        var items = await query
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
}

