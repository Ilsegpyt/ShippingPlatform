using BuildingBlocks.Application;
using MediatR;
using Operations.Application.Abstractions;
using Operations.Domain.Entities;
using Operations.Domain.Enums;

namespace Operations.Application.Operations.Ftl.AddFtlMaster;

public sealed class AddFtlMasterCommandHandler(
    IFtlAirportRepository airportRepository,
    IFtlTerminalRepository terminalRepository,
    IFtlTransporterRepository transporterRepository,
    IFtlDriverRepository driverRepository,
    IFtlVehicleRepository vehicleRepository,
    IDestinationRepository destinationRepository,
    IOperationsUnitOfWork unitOfWork)
    : IRequestHandler<AddFtlMasterCommand, Result<string>>
{
    public async Task<Result<string>> Handle(AddFtlMasterCommand command, CancellationToken ct)
    {
        var trimmedVal = command.Value?.Trim();
        if (string.IsNullOrWhiteSpace(trimmedVal))
            return Result.Failure<string>("Value cannot be empty.");

        var key = command.ListName?.Trim().ToLowerInvariant() ?? string.Empty;

        switch (key)
        {
            case "pod":
            case "airport":
            case "aod":
                if (!await airportRepository.NameExistsAsync(trimmedVal, null, ct))
                {
                    var airport = FtlAirport.Create(trimmedVal, trimmedVal.Length <= 5 ? trimmedVal.ToUpperInvariant() : null);
                    await airportRepository.AddAsync(airport, ct);
                    await unitOfWork.SaveChangesAsync(ct);
                }
                break;

            case "line":
            case "terminal":
                if (!await terminalRepository.NameExistsAsync(trimmedVal, null, ct))
                {
                    var terminal = FtlTerminal.Create(trimmedVal);
                    await terminalRepository.AddAsync(terminal, ct);
                    await unitOfWork.SaveChangesAsync(ct);
                }
                break;

            case "trucker":
            case "transporter":
                if (!await transporterRepository.NameExistsAsync(trimmedVal, null, ct))
                {
                    var transporter = FtlTransporter.Create(trimmedVal);
                    await transporterRepository.AddAsync(transporter, ct);
                    await unitOfWork.SaveChangesAsync(ct);
                }
                break;

            case "driver":
                if (!await driverRepository.NameOrNumberExistsAsync(trimmedVal, null, null, ct))
                {
                    var driver = FtlDriver.Create(trimmedVal);
                    await driverRepository.AddAsync(driver, ct);
                    await unitOfWork.SaveChangesAsync(ct);
                }
                break;

            case "cntr":
            case "vehicle":
            case "truck":
                if (!await vehicleRepository.PlateNumberExistsAsync(trimmedVal, null, ct))
                {
                    var vehicle = FtlVehicle.Create(trimmedVal, FtlTruckType.Jambo6m);
                    await vehicleRepository.AddAsync(vehicle, ct);
                    await unitOfWork.SaveChangesAsync(ct);
                }
                break;

            case "dest":
            case "destination":
                if (!await destinationRepository.DestinationNameExistsAsync(trimmedVal, null, ct))
                {
                    var dest = Destination.Create(trimmedVal);
                    await destinationRepository.AddAsync(dest, ct);
                    await unitOfWork.SaveChangesAsync(ct);
                }
                break;

            case "client":
            case "customer":
                // Client lookup is shared; acknowledge addition
                break;

            default:
                break;
        }

        return Result.Success(trimmedVal);
    }
}
