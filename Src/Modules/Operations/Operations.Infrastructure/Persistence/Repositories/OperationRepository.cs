using Microsoft.EntityFrameworkCore;
using Operations.Application.Abstractions;
using Operations.Domain.Entities;
using Operations.Domain.Enums;
using Operations.Application.Operations.SearchOperations;

namespace Operations.Infrastructure.Persistence.Repositories;

public sealed class OperationRepository(
    OperationsDbContext dbContext)
    : IOperationRepository
{
    public async Task AddAsync(
        Operation operation,
        CancellationToken cancellationToken = default)
    {
        await dbContext.Operations.AddAsync(operation, cancellationToken);
    }

    public Task<bool> ShipmentNumberExistsAsync(
        string shipmentNumber,
        Guid? excludeOperationId = null,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(shipmentNumber))
            return Task.FromResult(false);

        var clean = shipmentNumber.Trim();
        return dbContext.Operations.AnyAsync(
            x => !x.IsDeleted &&
                 x.ShipmentNumber != null &&
                 x.ShipmentNumber.Trim().ToLower() == clean.ToLower() &&
                 (!excludeOperationId.HasValue || x.Id != excludeOperationId.Value),
            cancellationToken);
    }

    public Task<bool> MblNumberExistsAsync(
        string mblNumber,
        Guid? excludeOperationId = null,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(mblNumber))
            return Task.FromResult(false);

        var clean = mblNumber.Trim();
        return dbContext.Operations.AnyAsync(
            x => !x.IsDeleted &&
                 x.OperationType == OperationType.Import &&
                 x.ImportDetails != null &&
                 x.ImportDetails.MBLNumber.Trim().ToLower() == clean.ToLower() &&
                 (!excludeOperationId.HasValue || x.Id != excludeOperationId.Value),
            cancellationToken);
    }

    public Task<bool> BookingNumberExistsAsync(
        string bookingNumber,
        Guid? excludeOperationId = null,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(bookingNumber))
            return Task.FromResult(false);

        var clean = bookingNumber.Trim();
        return dbContext.Operations.AnyAsync(
            x => !x.IsDeleted &&
                 x.OperationType == OperationType.Export &&
                 x.ExportDetails != null &&
                 x.ExportDetails.BookingNumber.Trim().ToLower() == clean.ToLower() &&
                 (!excludeOperationId.HasValue || x.Id != excludeOperationId.Value),
            cancellationToken);
    }

    public Task<bool> DuplicateCheckKeyExistsAsync(
        Operations.Domain.Enums.OperationType operationType,
        string duplicateCheckKey,
        Guid excludeOperationId,
        CancellationToken cancellationToken = default)
    {
        return dbContext.Operations.AnyAsync(
            x => x.OperationType == operationType &&
                 x.DuplicateCheckKey == duplicateCheckKey &&
                 x.Id != excludeOperationId,
            cancellationToken);
    }

    public Task<Operation?> GetByIdForUpdateAsync(
        Guid operationId,
        CancellationToken cancellationToken = default)
    {
        return dbContext.Operations
            .Include(x => x.ImportDetails)
            .Include(x => x.ExportDetails)
            .Include(x => x.Containers)
                .ThenInclude(x => x.Updates)
            .FirstOrDefaultAsync(x => x.Id == operationId, cancellationToken);
    }

    public Task<Operation?> GetByIdWithDetailsAsync(
        Guid operationId,
        CancellationToken cancellationToken = default)
    {
        return dbContext.Operations
            .AsNoTracking()
            .AsSplitQuery()
            .Include(x => x.ShippingLine)
            .Include(x => x.ImportDetails)
                .ThenInclude(x => x!.POD)
            .Include(x => x.ExportDetails)
                .ThenInclude(x => x!.POL)
            .Include(x => x.ExportDetails)
                .ThenInclude(x => x!.POD)
            .Include(x => x.ExportDetails)
                .ThenInclude(x => x!.POW)
            .Include(x => x.Containers)
                .ThenInclude(x => x.Transporter)
            .Include(x => x.Containers)
                .ThenInclude(x => x.Driver)
            .Include(x => x.Containers)
                .ThenInclude(x => x.Vehicle)
            .Include(x => x.Containers)
                .ThenInclude(x => x.Updates)
            .FirstOrDefaultAsync(x => x.Id == operationId, cancellationToken);
    }


    public async Task<IReadOnlyList<Operation>> GetDeliveredWithDetailsAsync(
        OperationType? operationType,
        CancellationToken cancellationToken = default)
    {
        var query = dbContext.Operations
            .AsNoTracking()
            .AsSplitQuery()
            .Where(x => x.Status == OperationStatus.Delivered);

        if (operationType.HasValue)
            query = query.Where(x => x.OperationType == operationType.Value);

        return await query
            .Include(x => x.ShippingLine)
            .Include(x => x.ImportDetails)
                .ThenInclude(x => x!.POD)
            .Include(x => x.ExportDetails)
                .ThenInclude(x => x!.POL)
            .Include(x => x.ExportDetails)
                .ThenInclude(x => x!.POD)
            .Include(x => x.ExportDetails)
                .ThenInclude(x => x!.POW)
            .Include(x => x.Containers)
                .ThenInclude(x => x.Transporter)
            .Include(x => x.Containers)
                .ThenInclude(x => x.Driver)
            .Include(x => x.Containers)
                .ThenInclude(x => x.Vehicle)
            .Include(x => x.Containers)
                .ThenInclude(x => x.Updates)
            .OrderByDescending(x => x.DeliveredAtUtc)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Operation>> GetDeletedWithDetailsAsync(
        CancellationToken cancellationToken = default)
    {
        return await dbContext.Operations
            .IgnoreQueryFilters()
            .AsNoTracking()
            .AsSplitQuery()
            .Where(x => x.IsDeleted)
            .OrderByDescending(x => x.DeletedAtUtc)
            .Include(x => x.ShippingLine)
            .Include(x => x.ImportDetails)
                .ThenInclude(x => x!.POD)
            .Include(x => x.ExportDetails)
                .ThenInclude(x => x!.POL)
            .Include(x => x.ExportDetails)
                .ThenInclude(x => x!.POD)
            .Include(x => x.ExportDetails)
                .ThenInclude(x => x!.POW)
            .Include(x => x.Containers)
                .ThenInclude(x => x.Transporter)
            .Include(x => x.Containers)
                .ThenInclude(x => x.Driver)
            .Include(x => x.Containers)
                .ThenInclude(x => x.Vehicle)
            .Include(x => x.Containers)
                .ThenInclude(x => x.Updates)
            .ToListAsync(cancellationToken);
    }


    public async Task<OperationSearchPage> SearchWithDetailsAsync(
        AdvancedOperationsSearchRequest request,
        CancellationToken cancellationToken = default)
    {
        var query = dbContext.Operations
            .AsNoTracking()
            .AsSplitQuery()
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.OperationType))
        {
            if (!Enum.TryParse<OperationType>(request.OperationType, true, out var operationType))
                throw new ArgumentException("Invalid operation type. Use Import or Export.");

            query = query.Where(x => x.OperationType == operationType);
        }

        if (!string.IsNullOrWhiteSpace(request.Status))
        {
            if (!Enum.TryParse<OperationStatus>(request.Status, true, out var status))
                throw new ArgumentException("Invalid status.");

            query = query.Where(x => x.Status == status);
        }

        query = AdvancedOperationQueryBuilder.Apply(
            query, request.Rules, request.SortBy, request.SortDirection);

        var totalCount = await query.CountAsync(cancellationToken);
        var page = Math.Max(1, request.Page);
        var pageSize = Math.Clamp(request.PageSize, 1, 200);

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Include(x => x.ShippingLine)
            .Include(x => x.ImportDetails)
                .ThenInclude(x => x!.POD)
            .Include(x => x.ExportDetails)
                .ThenInclude(x => x!.POL)
            .Include(x => x.ExportDetails)
                .ThenInclude(x => x!.POD)
            .Include(x => x.ExportDetails)
                .ThenInclude(x => x!.POW)
            .Include(x => x.Containers)
                .ThenInclude(x => x.Transporter)
            .Include(x => x.Containers)
                .ThenInclude(x => x.Driver)
            .Include(x => x.Containers)
                .ThenInclude(x => x.Vehicle)
            .Include(x => x.Containers)
                .ThenInclude(x => x.Updates)
            .ToListAsync(cancellationToken);

        return new OperationSearchPage(items, totalCount);
    }

    public async Task<IReadOnlyList<Operation>> SearchForExportAsync(
        AdvancedOperationsSearchRequest request,
        IReadOnlyCollection<Guid>? operationIds,
        DateTime? createdFromUtc,
        DateTime? createdToUtc,
        CancellationToken cancellationToken = default)
    {
        var query = dbContext.Operations
            .AsNoTracking()
            .AsSplitQuery()
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.OperationType))
        {
            if (!Enum.TryParse<OperationType>(request.OperationType, true, out var operationType))
                throw new ArgumentException("Invalid operation type. Use Import or Export.");
            query = query.Where(x => x.OperationType == operationType);
        }

        if (!string.IsNullOrWhiteSpace(request.Status))
        {
            if (!Enum.TryParse<OperationStatus>(request.Status, true, out var status))
                throw new ArgumentException("Invalid status.");
            query = query.Where(x => x.Status == status);
        }

        if (operationIds is { Count: > 0 })
            query = query.Where(x => operationIds.Contains(x.Id));

        if (createdFromUtc.HasValue)
            query = query.Where(x => x.CreatedAtUtc >= createdFromUtc.Value);

        if (createdToUtc.HasValue)
            query = query.Where(x => x.CreatedAtUtc < createdToUtc.Value);

        query = AdvancedOperationQueryBuilder.Apply(
            query, request.Rules, request.SortBy, request.SortDirection);

        return await query
            .Include(x => x.ShippingLine)
            .Include(x => x.ImportDetails).ThenInclude(x => x!.POD)
            .Include(x => x.ExportDetails).ThenInclude(x => x!.POL)
            .Include(x => x.ExportDetails).ThenInclude(x => x!.POD).Include(x => x.ExportDetails).ThenInclude(x => x!.POW)
            .Include(x => x.Containers).ThenInclude(x => x.Transporter)
            .Include(x => x.Containers).ThenInclude(x => x.Driver)
            .Include(x => x.Containers).ThenInclude(x => x.Vehicle)
            .Include(x => x.Containers).ThenInclude(x => x.Updates)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Operation>> GetAllWithDetailsAsync(
        bool onlyActive = false,
        CancellationToken cancellationToken = default)
    {
        var query = dbContext.Operations
            .AsNoTracking()
            .AsSplitQuery()
            .AsQueryable();

        if (onlyActive)
        {
            query = query.Where(x => x.Status != OperationStatus.Delivered);
        }

        return await query
            .Include(x => x.ShippingLine)
            .Include(x => x.ImportDetails)
                .ThenInclude(x => x!.POD)
            .Include(x => x.ExportDetails)
                .ThenInclude(x => x!.POL)
            .Include(x => x.ExportDetails)
                .ThenInclude(x => x!.POD)
            .Include(x => x.ExportDetails)
                .ThenInclude(x => x!.POW)
            .Include(x => x.Containers)
                .ThenInclude(x => x.Transporter)
            .Include(x => x.Containers)
                .ThenInclude(x => x.Driver)
            .Include(x => x.Containers)
                .ThenInclude(x => x.Vehicle)
            .Include(x => x.Containers)
                .ThenInclude(x => x.Updates)
            .OrderByDescending(x => x.CreatedAtUtc)
            .ToListAsync(cancellationToken);
    }

    public async Task<OperationsCountsDto> GetCountsAsync(
        CancellationToken cancellationToken = default)
    {
        var activeImport = await dbContext.Operations.CountAsync(
            x => !x.IsDeleted && x.Status != OperationStatus.Delivered && x.OperationType == OperationType.Import,
            cancellationToken);

        var activeExport = await dbContext.Operations.CountAsync(
            x => !x.IsDeleted && x.Status != OperationStatus.Delivered && x.OperationType == OperationType.Export,
            cancellationToken);

        var delivered = await dbContext.Operations.CountAsync(
            x => !x.IsDeleted && x.Status == OperationStatus.Delivered,
            cancellationToken);

        var inactive = await dbContext.Operations.IgnoreQueryFilters().CountAsync(
            x => x.IsDeleted,
            cancellationToken);

        return new OperationsCountsDto(activeImport, activeExport, delivered, inactive);
    }

    public async Task<PagedOperationsResult> GetPagedOperationsAsync(
        OperationQueryParams queryParams,
        CancellationToken cancellationToken = default)
    {
        var query = queryParams.IsDeleted
            ? dbContext.Operations.IgnoreQueryFilters().AsNoTracking().AsSplitQuery().Where(x => x.IsDeleted)
            : dbContext.Operations.AsNoTracking().AsSplitQuery().Where(x => !x.IsDeleted);

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

        // 1. Client filter (by GUID or name)
        if (!string.IsNullOrWhiteSpace(queryParams.Client))
        {
            var cVal = queryParams.Client.Trim();
            if (Guid.TryParse(cVal, out var cGuid))
            {
                query = query.Where(x => x.ClientId == cGuid);
            }
            else if (queryParams.MatchingFilterClientIds is { Count: > 0 } cIds)
            {
                query = query.Where(x => cIds.Contains(x.ClientId));
            }
            else
            {
                query = query.Where(x => false);
            }
        }

        // 2. Shipping Line filter (by GUID or name/abbreviation)
        if (!string.IsNullOrWhiteSpace(queryParams.Line))
        {
            var lVal = queryParams.Line.Trim();
            if (Guid.TryParse(lVal, out var lGuid))
            {
                query = query.Where(x => x.ShippingLineId == lGuid);
            }
            else
            {
                query = query.Where(x =>
                    (x.ShippingLine != null && EF.Functions.Like(x.ShippingLine.Name, $"%{lVal}%")) ||
                    (x.ShippingLine != null && x.ShippingLine.Abbreviation != null && EF.Functions.Like(x.ShippingLine.Abbreviation, $"%{lVal}%"))
                );
            }
        }

        // 3. Port filter (by GUID or name)
        if (!string.IsNullOrWhiteSpace(queryParams.Port))
        {
            var pVal = queryParams.Port.Trim();
            if (Guid.TryParse(pVal, out var pGuid))
            {
                query = query.Where(x =>
                    (x.ImportDetails != null && x.ImportDetails.PODId == pGuid) ||
                    (x.ExportDetails != null && (x.ExportDetails.POLId == pGuid || x.ExportDetails.PODId == pGuid || x.ExportDetails.POWId == pGuid))
                );
            }
            else
            {
                query = query.Where(x =>
                    (x.ImportDetails != null && x.ImportDetails.POD != null && EF.Functions.Like(x.ImportDetails.POD.Name, $"%{pVal}%")) ||
                    (x.ExportDetails != null && x.ExportDetails.POL != null && EF.Functions.Like(x.ExportDetails.POL.Name, $"%{pVal}%")) ||
                    (x.ExportDetails != null && x.ExportDetails.POD != null && EF.Functions.Like(x.ExportDetails.POD.Name, $"%{pVal}%")) ||
                    (x.ExportDetails != null && x.ExportDetails.POW != null && EF.Functions.Like(x.ExportDetails.POW.Name, $"%{pVal}%"))
                );
            }
        }

        // 4. Transporter filter (by GUID or name)
        if (!string.IsNullOrWhiteSpace(queryParams.Transporter))
        {
            var tVal = queryParams.Transporter.Trim();
            if (Guid.TryParse(tVal, out var tGuid))
            {
                query = query.Where(x => x.Containers.Any(c => c.TransporterId == tGuid));
            }
            else
            {
                query = query.Where(x => x.Containers.Any(c => c.Transporter != null && EF.Functions.Like(c.Transporter.Name, $"%{tVal}%")));
            }
        }

        // 5. Container Type filter
        if (!string.IsNullOrWhiteSpace(queryParams.ContainerType) &&
            Enum.TryParse<ContainerType>(queryParams.ContainerType.Trim(), ignoreCase: true, out var cntrEnum))
        {
            query = query.Where(x => x.Containers.Any(c => c.ContainerType == cntrEnum));
        }

        // 6. Destination filter
        if (!string.IsNullOrWhiteSpace(queryParams.Destination))
        {
            var dest = queryParams.Destination.Trim();
            query = query.Where(x => x.Containers.Any(c => c.Destination != null && EF.Functions.Like(c.Destination, $"%{dest}%")));
        }

        // 7. Clearance Type filter (CERT / BOSLA)
        if (!string.IsNullOrWhiteSpace(queryParams.ClearanceType) &&
            Enum.TryParse<ClearanceType>(queryParams.ClearanceType.Trim(), ignoreCase: true, out var clrEnum))
        {
            query = query.Where(x => x.ExportDetails != null && x.ExportDetails.ClearanceType == clrEnum);
        }

        // 8. Missing Docs filter ("yes" / "no")
        if (!string.IsNullOrWhiteSpace(queryParams.MissingDocs))
        {
            var md = queryParams.MissingDocs.Trim().ToLowerInvariant();
            if (md == "yes")
            {
                query = query.Where(x => x.Containers.Any(c => c.MissingDocs != null && c.MissingDocs.Trim() != ""));
            }
            else if (md == "no")
            {
                query = query.Where(x => x.Containers.All(c => c.MissingDocs == null || c.MissingDocs.Trim() == ""));
            }
        }

        // 9. Created By filter
        if (!string.IsNullOrWhiteSpace(queryParams.CreatedBy))
        {
            var cb = queryParams.CreatedBy.Trim();
            query = query.Where(x => x.CreatedByName != null && EF.Functions.Like(x.CreatedByName, $"%{cb}%"));
        }

        // 10. Customs filter
        if (!string.IsNullOrWhiteSpace(queryParams.Customs))
        {
            var cust = queryParams.Customs.Trim();
            query = query.Where(x => x.ExportDetails != null && x.ExportDetails.Customs != null && EF.Functions.Like(x.ExportDetails.Customs, $"%{cust}%"));
        }

        // 11. Date range filter
        if (!string.IsNullOrWhiteSpace(queryParams.DateFrom) || !string.IsNullOrWhiteSpace(queryParams.DateTo))
        {
            var hasFrom = DateOnly.TryParse(queryParams.DateFrom, out var dFrom);
            var hasTo = DateOnly.TryParse(queryParams.DateTo, out var dTo);
            var dtFromUtc = hasFrom ? dFrom.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc) : DateTime.MinValue;
            var dtToUtc = hasTo ? dTo.ToDateTime(TimeOnly.MaxValue, DateTimeKind.Utc) : DateTime.MaxValue;

            var dType = queryParams.DateType?.Trim().ToLowerInvariant() ?? "created";
            switch (dType)
            {
                case "created":
                    if (hasFrom) query = query.Where(x => x.CreatedAtUtc >= dtFromUtc);
                    if (hasTo) query = query.Where(x => x.CreatedAtUtc <= dtToUtc);
                    break;
                case "reqdate":
                    if (hasFrom) query = query.Where(x => (x.ImportDetails != null && x.ImportDetails.RequiredOffloadingDate >= dFrom) || x.Containers.Any(c => c.RequiredLoadingDate >= dFrom));
                    if (hasTo) query = query.Where(x => (x.ImportDetails != null && x.ImportDetails.RequiredOffloadingDate <= dTo) || x.Containers.Any(c => c.RequiredLoadingDate <= dTo));
                    break;
                case "arrdate":
                    if (hasFrom) query = query.Where(x => x.Containers.Any(c => c.ActualArrivalDate >= dFrom));
                    if (hasTo) query = query.Where(x => x.Containers.Any(c => c.ActualArrivalDate <= dTo));
                    break;
                case "gateout":
                    if (hasFrom) query = query.Where(x => x.Containers.Any(c => c.GateOutDate >= dFrom));
                    if (hasTo) query = query.Where(x => x.Containers.Any(c => c.GateOutDate <= dTo));
                    break;
                case "freeorcut":
                    if (hasFrom) query = query.Where(x => (x.ExportDetails != null && x.ExportDetails.CutOffDate >= dFrom) || x.Containers.Any(c => c.FreeTimeTill >= dFrom));
                    if (hasTo) query = query.Where(x => (x.ExportDetails != null && x.ExportDetails.CutOffDate <= dTo) || x.Containers.Any(c => c.FreeTimeTill <= dTo));
                    break;
                case "delivered":
                    if (hasFrom) query = query.Where(x => x.DeliveredAtUtc >= dtFromUtc);
                    if (hasTo) query = query.Where(x => x.DeliveredAtUtc <= dtToUtc);
                    break;
                case "checkout":
                    if (hasFrom) query = query.Where(x => x.Containers.Any(c => c.CheckOutFromThePort >= dtFromUtc));
                    if (hasTo) query = query.Where(x => x.Containers.Any(c => c.CheckOutFromThePort <= dtToUtc));
                    break;
                case "invrcv":
                    if (hasFrom) query = query.Where(x => x.InvoicesReceivedDate >= dtFromUtc);
                    if (hasTo) query = query.Where(x => x.InvoicesReceivedDate <= dtToUtc);
                    break;
                case "missrcv":
                    if (hasFrom) query = query.Where(x => x.Containers.Any(c => c.MissingDocsReceivedDate >= dFrom));
                    if (hasTo) query = query.Where(x => x.Containers.Any(c => c.MissingDocsReceivedDate <= dTo));
                    break;
            }
        }

        if (!string.IsNullOrWhiteSpace(queryParams.Search))
        {
            var term = queryParams.Search.Trim();
            var clientIds = queryParams.MatchingClientIds ?? [];

            query = query.Where(x =>
                (clientIds.Count > 0 && clientIds.Contains(x.ClientId)) ||
                (x.ShipmentNumber != null && EF.Functions.Like(x.ShipmentNumber, $"%{term}%")) ||
                (x.CertificateNumber != null && EF.Functions.Like(x.CertificateNumber, $"%{term}%")) ||
                (x.CreatedByName != null && EF.Functions.Like(x.CreatedByName, $"%{term}%")) ||
                (x.ShippingLine != null && EF.Functions.Like(x.ShippingLine.Name, $"%{term}%")) ||
                (x.ShippingLine != null && x.ShippingLine.Abbreviation != null && EF.Functions.Like(x.ShippingLine.Abbreviation, $"%{term}%")) ||
                (x.ImportDetails != null && EF.Functions.Like(x.ImportDetails.MBLNumber, $"%{term}%")) ||
                (x.ExportDetails != null && EF.Functions.Like(x.ExportDetails.BookingNumber, $"%{term}%")) ||
                (x.ExportDetails != null && x.ExportDetails.Customs != null && EF.Functions.Like(x.ExportDetails.Customs, $"%{term}%")) ||
                x.Containers.Any(c =>
                    (c.ContainerNumber != null && EF.Functions.Like(c.ContainerNumber, $"%{term}%")) ||
                    (c.Destination != null && EF.Functions.Like(c.Destination, $"%{term}%")) ||
                    (c.Driver != null && EF.Functions.Like(c.Driver.Name, $"%{term}%")) ||
                    (c.Driver != null && c.Driver.MobileNumber != null && EF.Functions.Like(c.Driver.MobileNumber, $"%{term}%")) ||
                    (c.Vehicle != null && c.Vehicle.CarNumber != null && EF.Functions.Like(c.Vehicle.CarNumber, $"%{term}%")) ||
                    (c.Transporter != null && EF.Functions.Like(c.Transporter.Name, $"%{term}%"))
                )
            );
        }

        var totalCount = await query.CountAsync(cancellationToken);
        var totalContainers = await query.SelectMany(x => x.Containers).CountAsync(cancellationToken);

        var page = Math.Max(1, queryParams.Page);
        var pageSize = Math.Clamp(queryParams.PageSize, 1, 200);
        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        var items = await query
            .OrderByDescending(x => x.CreatedAtUtc)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Include(x => x.ShippingLine)
            .Include(x => x.ImportDetails)
                .ThenInclude(x => x!.POD)
            .Include(x => x.ExportDetails)
                .ThenInclude(x => x!.POL)
            .Include(x => x.ExportDetails)
                .ThenInclude(x => x!.POD)
            .Include(x => x.ExportDetails)
                .ThenInclude(x => x!.POW)
            .Include(x => x.Containers)
                .ThenInclude(x => x.Transporter)
            .Include(x => x.Containers)
                .ThenInclude(x => x.Driver)
            .Include(x => x.Containers)
                .ThenInclude(x => x.Vehicle)
            .Include(x => x.Containers)
                .ThenInclude(x => x.Updates)
            .ToListAsync(cancellationToken);

        return new PagedOperationsResult(items, totalCount, totalContainers, page, pageSize, totalPages);
    }


    public Task<bool> ActiveShippingLineExistsAsync(
        Guid shippingLineId,
        CancellationToken cancellationToken = default)
    {
        return dbContext.ShippingLines.AnyAsync(
            x => x.Id == shippingLineId && x.IsActive,
            cancellationToken);
    }

    public Task<bool> ActivePortExistsAsync(
        Guid portId,
        CancellationToken cancellationToken = default)
    {
        return dbContext.Ports.AnyAsync(
            x => x.Id == portId && x.IsActive,
            cancellationToken);
    }
    public Task<bool> ActiveTransporterExistsAsync(
    Guid id,
    CancellationToken cancellationToken = default)
    {
        return dbContext.Transporters.AnyAsync(
            x => x.Id == id && x.IsActive,
            cancellationToken);
    }

    public Task<bool> ActiveDriverExistsAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return dbContext.Drivers.AnyAsync(
            x => x.Id == id && x.IsActive,
            cancellationToken);
    }

    public Task<bool> ActiveVehicleExistsAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return dbContext.Vehicles.AnyAsync(
            x => x.Id == id && x.IsActive,
            cancellationToken);
    }

    public Task<OperationContainer?> GetContainerByIdAsync(
        Guid containerId,
        CancellationToken cancellationToken = default)
    {
        return dbContext.OperationContainers
            .Include(x => x.Updates)
            .FirstOrDefaultAsync(x => x.Id == containerId, cancellationToken);
    }

    public async Task AddContainerUpdateAsync(
        ContainerUpdate update,
        CancellationToken cancellationToken = default)
    {
        await dbContext.ContainerUpdates.AddAsync(update, cancellationToken);
    }
}