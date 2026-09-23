using BuildingBlocks.Application;
using Customers.Application.Abstractions;
using Customers.Domain.Entities;
using MediatR;

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
    ICustomersUnitOfWork unitOfWork)
    : IRequestHandler<CreateCustomerVoiceCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(
        CreateCustomerVoiceCommand request,
        CancellationToken ct)
    {
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