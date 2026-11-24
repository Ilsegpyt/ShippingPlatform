using BuildingBlocks.Application;
using MediatR;
using Transporters.Application.Abstractions;
using Transporters.Application.Abstractions.Repositories;
using Transporters.Domain.Entities;

namespace Transporters.Application.ClientExceptions.AddClientException;

public sealed class AddClientExceptionCommandHandler(
    ITransporterRepository transporterRepository,
    ITransportersUnitOfWork transportersUnitOfWork)
    : IRequestHandler<AddClientExceptionCommand, Result>
{
    public async Task<Result> Handle(
        AddClientExceptionCommand cmd,
        CancellationToken ct)
    {
        var portCoverage = await transporterRepository.GetPortCoverageForUpdateAsync(
            cmd.PortCoverageId,
            ct);

        if (portCoverage is null || portCoverage.TransporterId != cmd.TransporterId)
            return Result.Failure("Port coverage was not found for this transporter.");

        var clientException = new TransporterClientException
        {
            TransporterId = cmd.TransporterId,
            CustomerId = cmd.CustomerId,
            PortCoverageId = cmd.PortCoverageId,
            Destination = cmd.Destination,
            EquipmentType = cmd.EquipmentType,
            AgreedPrice = cmd.AgreedPrice,
            ValidFromUtc = cmd.ValidFromUtc,
            ValidToUtc = cmd.ValidToUtc,
            Reason = cmd.Reason,
            IsActive = true
        };

        transporterRepository.AddClientException(clientException);

        await transportersUnitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }
}
