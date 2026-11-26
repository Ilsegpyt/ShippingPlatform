using BuildingBlocks.Application;
using BuildingBlocks.Application.Abstractions;
using CustomsClearance.Application.Abstractions;
using CustomsClearance.Application.Abstractions.Repositories;
using MediatR;

namespace CustomsClearance.Application.CustomsShipments.Inactive;

// Soft delete: moves an active (Open or Closed) shipment to the read-only Inactive archive.
// Status is kept so the archive can show the status before deactivation. There is no reactivate.
public sealed record DeactivateCustomsShipmentCommand(Guid Id) : IRequest<Result>;

public sealed class DeactivateCustomsShipmentCommandHandler(
    ICustomsShipmentRepository customsShipmentRepository,
    ICustomsClearanceUnitOfWork customsClearanceUnitOfWork,
    ICurrentUser currentUser,
    TimeProvider clock)
    : IRequestHandler<DeactivateCustomsShipmentCommand, Result>
{
    public async Task<Result> Handle(
        DeactivateCustomsShipmentCommand cmd,
        CancellationToken ct)
    {
        // Active shipments only (global query filter): an inactive one reads as not found.
        var shipment = await customsShipmentRepository.GetForUpdateAsync(cmd.Id, ct);

        if (shipment is null)
            return Result.Failure("Customs shipment was not found or is already inactive.");

        string? userId = null;
        string? userName = null;
        try
        {
            userId = currentUser.UserId == Guid.Empty ? null : currentUser.UserId.ToString();
            userName = await currentUser.GetUserNameAsync(ct);
        }
        catch (Exception ex) when (ex is InvalidOperationException or UnauthorizedAccessException)
        {
            // No resolvable user: the audit falls back to "User".
        }

        try
        {
            shipment.MarkAsInactive(userId, userName, clock.GetUtcNow().UtcDateTime);
        }
        catch (InvalidOperationException ex)
        {
            return Result.Failure(ex.Message);
        }

        await customsClearanceUnitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }
}
