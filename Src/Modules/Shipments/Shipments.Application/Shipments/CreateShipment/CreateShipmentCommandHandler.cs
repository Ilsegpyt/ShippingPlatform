using BuildingBlocks.Application;
using MediatR;
using Schedules.Contracts;
using Shipments.Application.Abstractions;
using Shipments.Domain.Declarations;
using Shipments.Domain.Shipments;

namespace Shipments.Application.Shipments.CreateShipment;

public sealed class CreateShipmentCommandHandler(
    IFileStorage fileStorage,
    IShipmentRepository shipmentRepository,
    IDeclarationFileRepository declarationFileRepository,
    IScheduleQueryService scheduleQueryService,
    IShipmentsUnitOfWork shipmentUnitOfWork)
    : IRequestHandler<
        CreateShipmentCommand,
        Result<CreateShipmentResponse>>
{
    public async Task<Result<CreateShipmentResponse>> Handle(
        CreateShipmentCommand command,
        CancellationToken ct)
    {
        var schedule = await scheduleQueryService.GetByIdAsync(
            command.ScheduleId,
            ct);

        if (schedule is null)
        {
            return Result.Failure<CreateShipmentResponse>(
                "Schedule was not found.");
        }

        var shipment = Shipment.Create(
            command.CustomerId,
            schedule.Id,
            schedule.Mode,
            schedule.Carrier,
            schedule.ContainerSize,
            command.Quantity,
            schedule.RateAmount);

        await shipmentRepository.AddAsync(
            shipment,
            ct);

        foreach (var file in command.DeclarationFiles)
        {
            var storageKey = await fileStorage.SaveAsync(
                file.Content,
                file.FileName,
                ct);

            var declarationFile = DeclarationFile.Create(
                shipment.Id,
                file.FileName,
                storageKey);

            await declarationFileRepository.AddAsync(
                declarationFile,
                ct);
        }

        await shipmentUnitOfWork.SaveChangesAsync(ct);

        return new CreateShipmentResponse(
            shipment.Id,
            shipment.ShipmentRef,
            shipment.Total);
    }
}