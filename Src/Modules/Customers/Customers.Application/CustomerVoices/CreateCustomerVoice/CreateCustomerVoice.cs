using BuildingBlocks.Application;
using Customers.Application.Abstractions;
using Customers.Domain.Entities;
using MediatR;
using Shipments.Contracts;

namespace Customers.Application.CustomerVoices.CreateCustomerVoice;

public sealed record CreateCustomerVoiceCommand(
    Guid CustomerId,
    Guid ShipmentId,
    Guid UserId,
    string Subject,
    string Message)
    : IRequest<Result<Guid>>;

public sealed class CreateCustomerVoiceCommandHandler(
    ICustomerVoiceRepository customerVoiceRepository,
    ICustomersUnitOfWork unitOfWork,
    IShipmentQueryService shipmentQueryService)
    : IRequestHandler<CreateCustomerVoiceCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(
        CreateCustomerVoiceCommand request,
        CancellationToken ct)
    {
        var shipment = await shipmentQueryService.GetByIdAsync(
            request.ShipmentId,
            ct);

        if (shipment is null)
            return Result.Failure<Guid>("Shipment not found.");

        if (shipment.CustomerId != request.CustomerId)
        {
            return Result.Failure<Guid>(
                "You are not allowed to create a Customer Voice for this shipment.");
        }

        var customerVoice = CustomerVoice.Create(
            request.CustomerId,
            request.ShipmentId,
            request.UserId,
            request.Subject,
            request.Message);

        await customerVoiceRepository.AddAsync(
            customerVoice,
            ct);

        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success(customerVoice.Id);
    }
}