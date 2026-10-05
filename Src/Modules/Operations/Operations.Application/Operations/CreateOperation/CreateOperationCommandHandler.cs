using BuildingBlocks.Application;
using MediatR;
using Operations.Application.Abstractions;
using Operations.Domain.Entities;

namespace Operations.Application.Operations.CreateOperation;

public sealed class CreateOperationCommandHandler(
    IOperationRepository operationRepository,
    IOperationsUnitOfWork unitOfWork)
    : IRequestHandler<CreateOperationCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(
        CreateOperationCommand command,
        CancellationToken ct)
    {
        var operation = new Operation(
            command.ClientId,
            command.ShippingLineId,
            command.OperationType,
            command.ShipmentNumber,
            command.CertificateNumber,
            command.InvoicesReceivedDate);

        await operationRepository.AddAsync(operation, ct);

        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success(operation.Id);
    }
}