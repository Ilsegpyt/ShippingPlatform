using BuildingBlocks.Application;
using BuildingBlocks.Application.Abstractions;
using CustomsClearance.Application.Abstractions;
using CustomsClearance.Application.Abstractions.Repositories;
using CustomsClearance.Domain.Entities;
using CustomsClearance.Domain.Enums;
using FluentValidation;
using MediatR;

namespace CustomsClearance.Application.CustomsShipments.Finance;

// Approve: the shipment leaves the finance request list and moves to Approved Requests.
public sealed record ApproveCustomsFinanceCommand(Guid ShipmentId, string? Note) : IRequest<Result>;

// Exclude: Defer (back on the request list on DeferredUntilDate) or OnHold (until restored by hand).
public sealed record ExcludeCustomsFinanceCommand(
    Guid ShipmentId,
    CustomsFinanceExcludeMode Mode,
    DateOnly? DeferredUntilDate,
    string? Note)
    : IRequest<Result>;

// Restore: an excluded (Deferred / OnHold) shipment goes back to the request list as Pending.
public sealed record RestoreCustomsFinanceCommand(Guid ShipmentId) : IRequest<Result>;

public sealed class ApproveCustomsFinanceCommandValidator : AbstractValidator<ApproveCustomsFinanceCommand>
{
    public ApproveCustomsFinanceCommandValidator()
    {
        RuleFor(x => x.ShipmentId).NotEmpty();
        RuleFor(x => x.Note).MaximumLength(500);
    }
}

public sealed class ExcludeCustomsFinanceCommandValidator : AbstractValidator<ExcludeCustomsFinanceCommand>
{
    public ExcludeCustomsFinanceCommandValidator()
    {
        RuleFor(x => x.ShipmentId).NotEmpty();
        RuleFor(x => x.Mode).IsInEnum();
        RuleFor(x => x.Note).MaximumLength(500);

        RuleFor(x => x.DeferredUntilDate)
            .NotNull()
            .When(x => x.Mode == CustomsFinanceExcludeMode.Defer)
            .WithMessage("Choose the date to defer the request to.");

        RuleFor(x => x.DeferredUntilDate)
            .Null()
            .When(x => x.Mode == CustomsFinanceExcludeMode.OnHold)
            .WithMessage("On hold has no defer date.");
    }
}

public sealed class RestoreCustomsFinanceCommandValidator : AbstractValidator<RestoreCustomsFinanceCommand>
{
    public RestoreCustomsFinanceCommandValidator()
    {
        RuleFor(x => x.ShipmentId).NotEmpty();
    }
}

public sealed class ApproveCustomsFinanceCommandHandler(
    ICustomsFinanceRepository customsFinanceRepository,
    ICustomsClearanceUnitOfWork customsClearanceUnitOfWork,
    ICurrentUser currentUser,
    TimeProvider clock)
    : IRequestHandler<ApproveCustomsFinanceCommand, Result>
{
    public async Task<Result> Handle(ApproveCustomsFinanceCommand cmd, CancellationToken ct)
    {
        if (!await customsFinanceRepository.IsOpenOceanImportAsync(cmd.ShipmentId, ct))
            return Result.Failure(CustomsFinanceRules.NotEligible);

        var request = await CustomsFinanceRules.GetOrAddAsync(customsFinanceRepository, cmd.ShipmentId, ct);
        if (request.Status == CustomsFinanceStatus.Approved)
            return Result.Failure("This finance request is already approved.");

        var now = clock.GetUtcNow().UtcDateTime;
        var userName = await CustomsFinanceRules.UserNameAsync(currentUser, ct);

        request.Status = CustomsFinanceStatus.Approved;
        request.DeferredUntilDate = null;
        request.Note = CustomsFinanceRules.Clean(cmd.Note);
        request.ApprovedAtUtc = now;
        request.ApprovedByName = userName;
        request.UpdatedAtUtc = now;
        request.UpdatedByName = userName;

        await customsClearanceUnitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }
}

public sealed class ExcludeCustomsFinanceCommandHandler(
    ICustomsFinanceRepository customsFinanceRepository,
    ICustomsClearanceUnitOfWork customsClearanceUnitOfWork,
    ICurrentUser currentUser,
    TimeProvider clock)
    : IRequestHandler<ExcludeCustomsFinanceCommand, Result>
{
    public async Task<Result> Handle(ExcludeCustomsFinanceCommand cmd, CancellationToken ct)
    {
        if (cmd.Mode == CustomsFinanceExcludeMode.Defer &&
            cmd.DeferredUntilDate <= CustomsCairoTime.Today(clock))
            return Result.Failure("The defer date must be after today (Cairo date).");

        if (!await customsFinanceRepository.IsOpenOceanImportAsync(cmd.ShipmentId, ct))
            return Result.Failure(CustomsFinanceRules.NotEligible);

        var request = await CustomsFinanceRules.GetOrAddAsync(customsFinanceRepository, cmd.ShipmentId, ct);

        var now = clock.GetUtcNow().UtcDateTime;

        request.Status = cmd.Mode == CustomsFinanceExcludeMode.Defer
            ? CustomsFinanceStatus.Deferred
            : CustomsFinanceStatus.OnHold;
        request.DeferredUntilDate = cmd.Mode == CustomsFinanceExcludeMode.Defer ? cmd.DeferredUntilDate : null;
        request.ApprovedAtUtc = null;
        request.ApprovedByName = null;
        request.Note = CustomsFinanceRules.Clean(cmd.Note);
        request.UpdatedAtUtc = now;
        request.UpdatedByName = await CustomsFinanceRules.UserNameAsync(currentUser, ct);

        await customsClearanceUnitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }
}

public sealed class RestoreCustomsFinanceCommandHandler(
    ICustomsFinanceRepository customsFinanceRepository,
    ICustomsClearanceUnitOfWork customsClearanceUnitOfWork,
    ICurrentUser currentUser,
    TimeProvider clock)
    : IRequestHandler<RestoreCustomsFinanceCommand, Result>
{
    public async Task<Result> Handle(RestoreCustomsFinanceCommand cmd, CancellationToken ct)
    {
        if (!await customsFinanceRepository.IsOpenOceanImportAsync(cmd.ShipmentId, ct))
            return Result.Failure(CustomsFinanceRules.NotEligible);

        var request = await customsFinanceRepository.GetForUpdateAsync(cmd.ShipmentId, ct);
        if (request is null ||
            request.Status is not (CustomsFinanceStatus.Deferred or CustomsFinanceStatus.OnHold))
            return Result.Failure("Only a deferred or on-hold finance request can be restored.");

        request.Status = CustomsFinanceStatus.Pending;
        request.DeferredUntilDate = null;
        request.Note = null;
        request.UpdatedAtUtc = clock.GetUtcNow().UtcDateTime;
        request.UpdatedByName = await CustomsFinanceRules.UserNameAsync(currentUser, ct);

        await customsClearanceUnitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }
}

internal static class CustomsFinanceRules
{
    public const string NotEligible = "Customs Finance applies only to open Ocean Import shipments.";

    public static async Task<CustomsFinanceRequest> GetOrAddAsync(
        ICustomsFinanceRepository repository,
        Guid shipmentId,
        CancellationToken ct)
    {
        var request = await repository.GetForUpdateAsync(shipmentId, ct);
        if (request is not null)
            return request;

        request = new CustomsFinanceRequest { ShipmentId = shipmentId, Status = CustomsFinanceStatus.Pending };
        repository.Add(request);
        return request;
    }

    public static async Task<string> UserNameAsync(ICurrentUser currentUser, CancellationToken ct)
    {
        try
        {
            var name = await currentUser.GetUserNameAsync(ct);
            return string.IsNullOrWhiteSpace(name) ? "User" : name.Trim();
        }
        catch (Exception ex) when (ex is InvalidOperationException or UnauthorizedAccessException)
        {
            return "User";
        }
    }

    public static string? Clean(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
