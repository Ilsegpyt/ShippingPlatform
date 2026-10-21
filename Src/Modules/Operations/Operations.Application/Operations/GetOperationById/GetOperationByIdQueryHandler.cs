using BuildingBlocks.Application;
using Customers.Contracts;
using MediatR;
using Operations.Application.Abstractions;
using Operations.Application.Operations.GetAllOperations;

namespace Operations.Application.Operations.GetOperationById;

public sealed class GetOperationByIdQueryHandler(
    IOperationRepository repository,
    ICustomerQueries customerQueries)
    : IRequestHandler<GetOperationByIdQuery, Result<OperationDto>>
{
    public async Task<Result<OperationDto>> Handle(
        GetOperationByIdQuery query,
        CancellationToken cancellationToken)
    {
        var operation = await repository.GetByIdWithDetailsAsync(
            query.Id,
            cancellationToken);

        if (operation is null)
            return Result.Failure<OperationDto>("Operation not found.");

        var customers = await customerQueries.GetByIdsAsync(
            new[] { operation.ClientId },
            cancellationToken);
        var clientName = customers.FirstOrDefault()?.CompanyName;

        var response = new OperationDto(
            operation.Id,
            operation.CreatedAtUtc,
            operation.CreatedByName,
            operation.ClientId,
            clientName,
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
                        .ToList()))
                .ToList());

        return Result.Success(response);
    }
}
