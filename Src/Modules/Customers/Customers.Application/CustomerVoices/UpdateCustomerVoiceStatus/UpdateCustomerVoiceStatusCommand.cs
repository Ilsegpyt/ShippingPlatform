using BuildingBlocks.Application;
using Customers.Application.Abstractions;
using Customers.Domain.Entities;
using MediatR;

namespace Customers.Application.CustomerVoices.UpdateCustomerVoiceStatus;

public sealed record UpdateCustomerVoiceStatusCommand(
    Guid CustomerVoiceId,
    CustomerVoiceStatus Status)
    : IRequest<Result>;

public sealed class UpdateCustomerVoiceStatusCommandHandler(
    ICustomerVoiceRepository customerVoiceRepository,
    ICustomersUnitOfWork unitOfWork)
    : IRequestHandler<UpdateCustomerVoiceStatusCommand, Result>
{
    public async Task<Result> Handle(
        UpdateCustomerVoiceStatusCommand request,
        CancellationToken ct)
    {
        var customerVoice =
            await customerVoiceRepository.GetByIdAsync(
                request.CustomerVoiceId,
                ct);

        if (customerVoice is null)
            return Result.Failure("Customer Voice not found.");

        customerVoice.ChangeStatus(request.Status);

        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }
}