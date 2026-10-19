
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
    IOperationRepository operationRepository,
    IOperationsUnitOfWork unitOfWork,
    ICurrentUser currentUser)
    : IRequestHandler<ApproveFclSuggestionCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(
        ApproveFclSuggestionCommand command,
        CancellationToken ct)
    {
        var suggestion = await fclRepository
            .GetSuggestionForApprovalAsync(command.SuggestionId, ct);

        if (suggestion is null)
        {
            return Result.Failure<Guid>(
                "FCL suggestion not found.");
        }

        if (suggestion.Status != FclSuggestionStatus.Pending)
        {
            return Result.Failure<Guid>(
                "This FCL suggestion has already been processed.");
        }

        // Only a perfect match can be approved.
        if (suggestion.MatchCount != 6)
        {
            return Result.Failure<Guid>(
                $"This suggestion matches {suggestion.MatchCount}/6 criteria. " +
                "Only 6/6 suggestions can be approved.");
        }

        var import = suggestion.ImportOperation;
        var importContainer = suggestion.ImportContainer;
        var sourceExportContainer = suggestion.ExportContainer;
        var sourceExport = sourceExportContainer?.Operation;

        if (sourceExport is null ||
            sourceExport.OperationType != OperationType.Export ||
            sourceExport.Status != OperationStatus.Open ||
            sourceExport.ExportDetails is null)
        {
            return Result.Failure<Guid>(
                "The source Export container or its open operation was not found.");
        }

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

        string approvalKey;

        try
        {
            approvalKey = FclApprovalKey.Create(
                importContainer.ContainerNumber,
                import.ImportDetails.MBLNumber,
                suggestion.BookingNumber);
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

        Operation newExport;

        try
        {
            var sourceDetails = sourceExport.ExportDetails;

            newExport = new Operation(
                sourceExport.ClientId,
                sourceExport.ShippingLineId,
                OperationType.Export,
                sourceExport.ShipmentNumber,
                sourceExport.CertificateNumber,
                sourceExport.InvoicesReceivedDate);

            newExport.MarkAsFcl();

            newExport.SetExportDetails(new ExportDetails(
                sourceDetails.ClearanceType,
                sourceDetails.BookingNumber,
                sourceDetails.POLId,
                sourceDetails.POWId,
                sourceDetails.CutOffDate));

            // Create exactly one container in the new Export operation.
            var newContainer = new OperationContainer(
                newExport.Id,
                1,
                importContainer.ContainerNumber,
                importContainer.ContainerType.Value,
                suggestion.RequiredLoadingDate,
                suggestion.RequiredLoadingTime);

            newContainer.UpdateExportData(
                importContainer.ContainerType,
                importContainer.ContainerNumber,
                suggestion.RequiredLoadingDate,
                suggestion.RequiredLoadingTime,
                sourceExportContainer.Destination);

            newExport.Containers.Add(newContainer);

            suggestion.MarkApproved();
        }
        catch (InvalidOperationException ex)
        {
            return Result.Failure<Guid>(ex.Message);
        }
        catch (ArgumentException ex)
        {
            return Result.Failure<Guid>(ex.Message);
        }

        var approval = new FclApproval(
            suggestion.Id,
            newExport.Id,
            approvalKey,
            currentUser.UserId.ToString(),
            DateTime.UtcNow);

        try
        {
            await unitOfWork.BeginTransactionAsync(ct);

            await operationRepository.AddAsync(newExport, ct);
            await fclRepository.AddApprovalAsync(approval, ct);

            // Persist the new Export, approval, and suggestion status together.
            await unitOfWork.SaveChangesAsync(ct);

            await unitOfWork.CommitTransactionAsync(ct);

            return Result.Success(approval.Id);
        }
        catch
        {
            await unitOfWork.RollbackTransactionAsync(ct);
            throw;
        }
    }
}
