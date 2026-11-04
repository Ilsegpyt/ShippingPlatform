using BuildingBlocks.Application;
using Customers.Contracts;
using MediatR;
using Operations.Application.Abstractions;
using Operations.Application.Operations.GetAllOperations;

namespace Operations.Application.Operations.GetPagedOperations;

public sealed class GetPagedOperationsQueryHandler(
    IOperationRepository repository,
    ICustomerQueries customerQueries)
    : IRequestHandler<GetPagedOperationsQuery, Result<PagedOperationsResponseDto>>
{
    public async Task<Result<PagedOperationsResponseDto>> Handle(
        GetPagedOperationsQuery query,
        CancellationToken cancellationToken)
    {
        IReadOnlyCollection<Guid>? matchingClientIds = null;
        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var search = query.Search.Trim();
            var allCustomers = await customerQueries.GetAllForOperationsAsync(cancellationToken);
            matchingClientIds = allCustomers
                .Where(c => c.CompanyName.Contains(search, StringComparison.OrdinalIgnoreCase))
                .Select(c => c.CustomerId)
                .ToList();
        }

        IReadOnlyCollection<Guid>? matchingFilterClientIds = null;
        if (!string.IsNullOrWhiteSpace(query.Client) && !Guid.TryParse(query.Client.Trim(), out _))
        {
            var clientName = query.Client.Trim();
            var allCustomers = await customerQueries.GetAllForOperationsAsync(cancellationToken);
            matchingFilterClientIds = allCustomers
                .Where(c => c.CompanyName.Contains(clientName, StringComparison.OrdinalIgnoreCase))
                .Select(c => c.CustomerId)
                .ToList();
        }

        var queryParams = new OperationQueryParams(
            Page: query.Page,
            PageSize: query.PageSize,
            OperationType: query.OperationType,
            Status: query.Status,
            OnlyActive: query.OnlyActive,
            IsDeleted: query.IsDeleted,
            Search: query.Search,
            Client: query.Client,
            Line: query.Line,
            Port: query.Port,
            Transporter: query.Transporter,
            ContainerType: query.ContainerType,
            Destination: query.Destination,
            ClearanceType: query.ClearanceType,
            MissingDocs: query.MissingDocs,
            CreatedBy: query.CreatedBy,
            Customs: query.Customs,
            DateType: query.DateType,
            DateFrom: query.DateFrom,
            DateTo: query.DateTo,
            MatchingClientIds: matchingClientIds,
            MatchingFilterClientIds: matchingFilterClientIds);

        var pagedResult = await repository.GetPagedOperationsAsync(queryParams, cancellationToken);

        var customerIds = pagedResult.Items
            .Select(operation => operation.ClientId)
            .Distinct()
            .ToArray();
        var customers = await customerQueries.GetByIdsAsync(customerIds, cancellationToken);
        var clientNames = customers.ToDictionary(customer => customer.CustomerId, customer => customer.CompanyName);

        IReadOnlyList<OperationDto> response = pagedResult.Items
            .Select(operation => new OperationDto(
                operation.Id,
                operation.CreatedAtUtc,
                operation.CreatedByName,
                operation.ClientId,
                clientNames.GetValueOrDefault(operation.ClientId),
                operation.ShippingLineId,
                operation.ShippingLine.Name,
                operation.ShippingLine.Abbreviation,
                operation.OperationType.ToString(),
                operation.Status.ToString(),
                operation.Source.ToString(),
                operation.ShipmentNumber,
                operation.CertificateNumber,
                operation.InvoicesReceivedDate,
                operation.DeliveredAtUtc,
                operation.DeliveredByUserId,
                operation.ReopenedAtUtc,
                operation.ReopenedByUserId,
                operation.ReopenReason,
                operation.NumberOfContainers,

                operation.ImportDetails is { } importDetails
                    ? new ImportDetailsDto(
                        importDetails.Id,
                        importDetails.MBLNumber,
                        importDetails.PODId,
                        importDetails.POD.Name,
                        importDetails.POD.Code,
                        importDetails.RequiredOffloadingDate,
                        importDetails.RequiredOffloadingTime)
                    : null,

                operation.ExportDetails is { } exportDetails
                    ? new ExportDetailsDto(
                        exportDetails.Id,
                        exportDetails.ClearanceType.ToString(),
                        exportDetails.BookingNumber,
                        exportDetails.POLId,
                        exportDetails.POL.Name,
                        exportDetails.POL.Code,
                        exportDetails.POWId,
                        exportDetails.POW?.Name,
                        exportDetails.POW?.Code,
                        exportDetails.CutOffDate,
                        exportDetails.Customs,
                        exportDetails.PODId,
                        exportDetails.POD?.Name,
                        exportDetails.POD?.Code)
                    : null,

                operation.Containers
                    .OrderBy(container => container.Ordinal)
                    .Select(container => new OperationContainerDto(
                        container.Id,
                        container.OperationId,
                        container.Ordinal,
                        container.ContainerNumber,
                        container.ContainerType?.ToString(),
                        container.Destination,
                        container.FreeTimeTill,
                        container.RequiredLoadingDate,
                        container.RequiredLoadingTime,
                        container.ActualArrivalDate,
                        container.ActualArrivalTime,
                        container.GateOutDate,
                        container.GateOutTime,
                        container.MissingDocs,
                        container.MissingDocsReceivedDate,
                        container.CheckOutFromThePort,
                        container.TransporterId,
                        container.Transporter?.Name,
                        container.DriverId,
                        container.Driver?.DriverNumber,
                        container.Driver?.Name,
                        container.Driver?.MobileNumber,
                        container.VehicleId,
                        container.Vehicle?.CarNumber,

                        container.Updates
                            .OrderByDescending(update => update.CreatedAtUtc)
                            .Select(update => new ContainerUpdateDto(
                                update.Id,
                                update.OperationContainerId,
                                update.Details,
                                update.CreatedAtUtc,
                                update.CreatedByUserId,
                                update.CreatedByName))
                            .ToList()
                    ))
                    .ToList(),

                operation.IsDeleted,
                operation.DeletedAtUtc,
                operation.DeletedByUserId,
                operation.DeletedByName
            ))
            .ToList();

        return Result.Success(new PagedOperationsResponseDto(
            response,
            pagedResult.TotalCount,
            pagedResult.TotalContainersCount,
            pagedResult.Page,
            pagedResult.PageSize,
            pagedResult.TotalPages));
    }
}
