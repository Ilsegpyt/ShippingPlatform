namespace CustomsClearance.Application.CustomsShipments;

// Calendar days in the module follow Cairo time (Africa/Cairo, DST-aware).
public static class CustomsCairoTime
{
    public static readonly TimeZoneInfo Zone = Find();

    private static TimeZoneInfo Find()
    {
        foreach (var id in new[] { "Africa/Cairo", "Egypt Standard Time" })
            if (TimeZoneInfo.TryFindSystemTimeZoneById(id, out var tz))
                return tz;

        return TimeZoneInfo.CreateCustomTimeZone("Cairo", TimeSpan.FromHours(2), "Cairo", "Cairo");
    }

    public static DateOnly Today(TimeProvider clock) =>
        DateOnly.FromDateTime(TimeZoneInfo.ConvertTime(clock.GetUtcNow(), Zone).DateTime);

    // UTC instant at which the given Cairo calendar day starts.
    public static DateTime DayStartUtc(DateOnly day) =>
        TimeZoneInfo.ConvertTimeToUtc(
            DateTime.SpecifyKind(day.ToDateTime(TimeOnly.MinValue), DateTimeKind.Unspecified),
            Zone);
}
