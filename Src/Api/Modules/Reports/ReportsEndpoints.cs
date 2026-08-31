
namespace Api.Modules.Reports;

public static class ReportsEndpoints
{
    public static void MapReportsEndpoints(this IEndpointRouteBuilder app)
    {
        UploadReportEndpoint.Map(app);
        GetReportsEndpoint.Map(app);
        DownloadReportEndpoint.Map(app);
    }
}