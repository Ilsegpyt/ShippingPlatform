using Identity.Domain;
using Identity.Infrastructure.Authorization;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Schedules.Application.Schedules.UpdateSchedule;
using Schedules.Domain.Schedule;

namespace Api.Modules.Schedules;

public static class UpdateScheduleEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPatch("/api/schedules/{id:guid}", async (
            Guid id,
            [FromBody] UpdateScheduleRequest request,
            ISender sender,
            CancellationToken ct) =>
        {
            var command = new UpdateScheduleCommand(
                id,
                request.RouteId,
                request.Mode,
                request.DepartureDate,
                request.Vessel,
                request.Origin,
                request.DeparturePortCode,
                request.DepartureCountry,
                request.Destination,
                request.ArrivalPortCode,
                request.ArrivalCountry,
                request.Carrier,
                request.CarrierCode,
                request.VoyageNumber,
                request.Arrival,
                request.TransitTime,
                request.CutoffDate,
                request.RateCurrency,
                request.ContainerSize,
                request.RateAmount,
                request.RateRemarks,
                request.ValidityDate,
                request.FreeTimeAtPOD,
                request.FreeTimeAtPOL,
                request.TransshipmentData,
                request.Notes);

            var result = await sender.Send(command, ct);

            return result.IsSuccess
                ? Results.NoContent()
                : Results.BadRequest(result.Error);
        }).RequirePermission(PermissionCatalog.SchedulesUpdate);
    }
}

public sealed record UpdateScheduleRequest(
    string? RouteId = null,
    ScheduleMode? Mode = null,
    DateOnly? DepartureDate = null,
    string? Vessel = null,
    string? Origin = null,
    string? DeparturePortCode = null,
    string? DepartureCountry = null,
    string? Destination = null,
    string? ArrivalPortCode = null,
    string? ArrivalCountry = null,
    string? Carrier = null,
    string? CarrierCode = null,
    string? VoyageNumber = null,
    DateOnly? Arrival = null,
    TimeSpan? TransitTime = null,
    DateOnly? CutoffDate = null,
    string? RateCurrency = null,
    ContainerSize? ContainerSize = null,
    decimal? RateAmount = null,
    string? RateRemarks = null,
    DateOnly? ValidityDate = null,
    int? FreeTimeAtPOD = null,
    int? FreeTimeAtPOL = null,
    string? TransshipmentData = null,
    string? Notes = null);