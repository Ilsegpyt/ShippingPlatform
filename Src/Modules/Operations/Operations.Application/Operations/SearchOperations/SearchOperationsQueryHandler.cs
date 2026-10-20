using BuildingBlocks.Application;
using MediatR;
using Operations.Application.Abstractions;
using Operations.Application.Operations.GetAllOperations;

namespace Operations.Application.Operations.SearchOperations;

public sealed class SearchOperationsQueryHandler(
    IOperationRepository repository)
    : IRequestHandler<SearchOperationsQuery, Result<AdvancedOperationsSearchResponse<OperationDto>>>
{
    public async Task<Result<AdvancedOperationsSearchResponse<OperationDto>>> Handle(
        SearchOperationsQuery query,
        CancellationToken cancellationToken)
    {
        OperationSearchPage pageResult;
        try
        {
            pageResult = await repository.SearchWithDetailsAsync(query.Request, cancellationToken);
        }
        catch (ArgumentException ex)
        {
            return Result.Failure<AdvancedOperationsSearchResponse<OperationDto>>(ex.Message);
        }

        IReadOnlyList<OperationDto> response = pageResult.Items
            .Select(operation => new OperationDto(
                operation.Id,
                operation.CreatedAtUtc,
                operation.CreatedByName,
                operation.ClientId,
                operation.ShippingLineId,
                operation.ShippingLine.Name,
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
                        exportDetails.Customs)
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
                    .ToList()
            ))
            .ToList();


        var page = Math.Max(1, query.Request.Page);
        var pageSize = Math.Clamp(query.Request.PageSize, 1, 200);
        return Result.Success(new AdvancedOperationsSearchResponse<OperationDto>(
            response, pageResult.TotalCount, page, pageSize));
    }
}
