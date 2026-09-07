using MediatR;
using Reports.Domain.Entities;

namespace Reports.Application.Reports.GetReports;

public sealed record GetReportsQuery(
    Guid UserId)
    : IRequest<IReadOnlyList<Report>>;