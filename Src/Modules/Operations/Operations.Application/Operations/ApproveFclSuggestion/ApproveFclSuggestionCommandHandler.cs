
using BuildingBlocks.Application;
using BuildingBlocks.Application.Abstractions;
using MediatR;
using Operations.Application.Abstractions;
using Operations.Application.Common;
using Operations.Domain.Entities;
using Operations.Domain.Enums;

namespace Operations.Application.Operations.ApproveFclSuggestion;

public sealed class ApproveFclSuggestionCommandHandler(
    IFclRepository fclRepository,
    IOperationsUnitOfWork unitOfWork,
    ICurrentUser currentUser)
    : IRequestHandler<ApproveFclSuggestionCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(
        ApproveFclSuggestionCommand command,
        CancellationToken ct)
    {
        // 1. Resolve import container directly
        var importContainer = await fclRepository
            .GetImportContainerForApprovalAsync(command.SuggestionId, ct);

        if (importContainer is null)
        {
            return Result.Failure<Guid>(
                "Import container not found.");
        }

        // 2. Ensure container is not already approved
        if (await fclRepository.IsImportContainerApprovedAsync(importContainer.Id, ct))
        {
            return Result.Failure<Guid>(
                "This import container has already been approved.");
        }

        var import = importContainer.Operation;
        if (import.ImportDetails is null ||
            string.IsNullOrWhiteSpace(import.ImportDetails.MBLNumber))
        {
            return Result.Failure<Guid>(
                "The Import operation has no valid M/BL number.");
        }

        if (string.IsNullOrWhiteSpace(importContainer.ContainerNumber) ||
            !importContainer.ContainerType.HasValue)
        {
            return Result.Failure<Guid>(
                "The Import container has incomplete container data.");
        }

        // 3. Resolve booking number and export operation ID
        string? bookingNumber = command.BookingNumber;
        Guid? exportOperationId = command.ExportOperationId;

        if (string.IsNullOrWhiteSpace(bookingNumber))
        {
            var exportContainers = await fclRepository.GetAllExportContainersForMatchingAsync(ct);
            var (matchedExportContainer, _) = FclMatchingEngine.FindBestExportMatch(importContainer, exportContainers);

            if (matchedExportContainer is not null)
            {
                bookingNumber = matchedExportContainer.Operation.ExportDetails?.BookingNumber;
                exportOperationId ??= matchedExportContainer.Operation.Id;
            }
        }

        bookingNumber = !string.IsNullOrWhiteSpace(bookingNumber)
            ? bookingNumber
            : "APPROVED";

        // 4. Create and validate approval key
        string approvalKey;
        try
        {
            approvalKey = FclApprovalKey.Create(
                importContainer.ContainerNumber,
                import.ImportDetails.MBLNumber,
                bookingNumber);
        }
        catch (ArgumentException ex)
        {
            return Result.Failure<Guid>(ex.Message);
        }

        if (await fclRepository.ApprovalKeyExistsAsync(approvalKey, ct))
        {
            return Result.Failure<Guid>(
                "This container, M/BL and booking combination has already been approved.");
        }

        // 5. Persist approval only without creating Export or any other entity
        var approval = new FclApproval(
            importContainer.Id,
            approvalKey,
            currentUser.UserId.ToString(),
            DateTime.UtcNow,
            exportOperationId);

        await fclRepository.AddApprovalAsync(approval, ct);
        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success(approval.Id);
    }
}
