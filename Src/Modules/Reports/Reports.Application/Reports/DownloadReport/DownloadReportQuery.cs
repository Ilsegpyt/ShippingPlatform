using MediatR;

namespace Reports.Application.Reports.DownloadReport;

public sealed record DownloadReportQuery(
    Guid ReportId,
    Guid UserId)
    : IRequest<DownloadReportResult?>;

public sealed record DownloadReportResult(
    string FileName,
    string StorageKey);