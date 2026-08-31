using BuildingBlocks.Application.Contracts;
using MediatR;
using Reports.Domain.Report;

namespace Reports.Application.Reports.GetReports;

public sealed record GetReportsQuery(
    Guid UserId)
    : IRequest<IReadOnlyList<Report>>;