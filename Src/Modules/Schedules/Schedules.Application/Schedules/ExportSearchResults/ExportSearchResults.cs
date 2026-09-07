using BuildingBlocks.Application;
using MediatR;
using Schedules.Domain.Entities;
using Schedules.Domain.Enums;

namespace Schedules.Application.Schedules.ExportSearchResults;

public sealed record ExportSearchResultsQuery(
    string Origin,
    string Destination,
    DateOnly DepartureDate,
    ContainerSize ContainerSize
) : IRequest<Result<IReadOnlyList<Schedule>>>;