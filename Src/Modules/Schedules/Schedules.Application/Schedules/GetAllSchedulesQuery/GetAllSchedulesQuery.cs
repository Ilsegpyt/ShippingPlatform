using BuildingBlocks.Application;
using MediatR;
using Schedules.Application.Abstractions;
using Schedules.Domain.Enums;

namespace Schedules.Application.Queries.GetAllSchedules;

public sealed record GetAllSchedulesQuery(PaginationRequest Pagination)
    : IRequest<PagedResult<ScheduleResponse>>;

public sealed record ScheduleResponse(
    Guid Id,
    string? RouteId,
    ScheduleMode Mode,
    DateOnly DepartureDate,
    string Vessel,
    string Origin,
    string DeparturePortCode,
    string DepartureCountry,
    string Destination,
    string ArrivalPortCode,
    string ArrivalCountry,
    string Carrier,
    string CarrierCode,
    string VoyageNumber,
    DateOnly Arrival,
    TimeSpan TransitTime,
    DateOnly CutoffDate,
    DateOnly PortCutoffDate,
    string RateCurrency,
    ContainerSize ContainerSize,
    decimal RateAmount,
    string? RateRemarks,
    DateOnly ValidityDate,
    int FreeTimeAtPOD,
    int FreeTimeAtPOL,
    string? TransshipmentData,
    string? Notes,
    DateTime CreatedAtUtc,
    DateTime? UpdatedAtUtc);

public sealed class GetAllSchedulesQueryHandler(IScheduleRepository repository)
    : IRequestHandler<GetAllSchedulesQuery, PagedResult<ScheduleResponse>>
{
    public async Task<PagedResult<ScheduleResponse>> Handle(
        GetAllSchedulesQuery query,
        CancellationToken ct)
    {
        var page = query.Pagination.PageNumber;
        var pageSize = query.Pagination.PageSize;
        var skip = (page - 1) * pageSize;

        var totalCount = await repository.CountAsync(ct);

        var schedules = await repository.GetAllAsync(
            skip,
            pageSize,
            ct);

        var items = schedules
            .Select(x => new ScheduleResponse(
                x.Id,
                x.RouteId,
                x.Mode,
                x.DepartureDate,
                x.Vessel,
                x.Origin,
                x.DeparturePortCode,
                x.DepartureCountry,
                x.Destination,
                x.ArrivalPortCode,
                x.ArrivalCountry,
                x.Carrier,
                x.CarrierCode,
                x.VoyageNumber,
                x.Arrival,
                x.TransitTime,
                x.CutoffDate,
                x.PortCutoffDate,
                x.RateCurrency,
                x.ContainerSize,
                x.RateAmount,
                x.RateRemarks,
                x.ValidityDate,
                x.FreeTimeAtPOD,
                x.FreeTimeAtPOL,
                x.TransshipmentData,
                x.Notes,
                x.CreatedAtUtc,
                x.UpdatedAtUtc))
            .ToList();

        return new PagedResult<ScheduleResponse>(
            items,
            totalCount,
            page,
            pageSize);
    }
}