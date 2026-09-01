namespace Api.Modules.Schedules;

public static class SchedulesEndpoints
{
    public static void MapSchedulesEndpoints(
        this IEndpointRouteBuilder app)
    {
        CreateScheduleEndpoint.Map(app);
        SearchSchedulesEndpoint.Map(app);
        ImportSchedulesEndpoint.Map(app);
        DeleteScheduleEndpoint.Map(app);
        ExportSchedulesEndpoint.Map(app);
        ExportSearchResultsEndpoint.Map(app);
        BulkDeleteSchedulesEndpoint.Map(app);
        UpdateScheduleEndpoint.Map(app);
    }
}
