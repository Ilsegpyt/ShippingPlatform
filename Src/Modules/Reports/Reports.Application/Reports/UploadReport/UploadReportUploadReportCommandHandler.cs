using BuildingBlocks.Application;
using BuildingBlocks.Application.Contracts;
using MediatR;
using Reports.Application.Abstractions;
using Reports.Domain.Report;

namespace Reports.Application.Reports.UploadReport;

public sealed class UploadReportCommandHandler(
    IReportRepository reportRepository,
    IReportsUnitOfWork unitOfWork,
    IReportFileStorage fileStorage,
    IAccountManagerQueries accountManagerQueries,
    ICustomerQueries customerQueries)
    : IRequestHandler<UploadReportCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(
        UploadReportCommand command,
        CancellationToken ct)
    {
        var customer =
            await customerQueries.GetForAssignmentAsync(
                command.CustomerId,
                ct);

        if (customer is null)
            return Result.Failure<Guid>(
                "Customer was not found.");

        if (!customer.IsActive)
            return Result.Failure<Guid>(
                "Customer is inactive.");

        var isAssigned =
            await accountManagerQueries.IsAssignedToCustomerAsync(
                command.UploadedByUserId,
                command.CustomerId,
                ct);

        if (!isAssigned)
            return Result.Failure<Guid>(
                "The Account Manager is not assigned to this Customer.");

        if (!ReportClassification.IsValid(
                command.Category,
                command.Service,
                command.ShipmentType))
        {
            return Result.Failure<Guid>(
                "Invalid report classification.");
        }

        var storageKey = await fileStorage.SaveAsync(
            command.File,
            command.FileName,
            ct);

        var report = Report.Create(
            command.CustomerId,
            command.ShipmentRef,
            command.Category,
            command.Service,
            command.ShipmentType,
            command.FileName,
            storageKey,
            command.UploadedByUserId);

        await reportRepository.AddAsync(report, ct);

        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success(report.Id);
    }
}