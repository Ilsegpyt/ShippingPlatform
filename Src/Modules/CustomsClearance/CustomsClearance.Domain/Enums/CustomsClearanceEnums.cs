namespace CustomsClearance.Domain.Enums;

public enum ReportType
{
    OceanImport = 1,
    OceanExport = 2,
    Domestic = 3
}

public enum CustomsShipmentStatus
{
    Open = 1,
    Closed = 2
}

public enum OceanImportClearanceType
{
    Import = 1,
    Transit = 2
}

public enum OceanExportClearanceType
{
    Certificate = 1,
    Bosla = 2
}

// Ocean import customs track (stored as text, nullable). Inspection date applies only to Red and GreenImports.
public static class OceanImportTrack
{
    public const string Blue = "Blue";
    public const string Yellow = "Yellow";
    public const string Green = "Green";
    public const string Red = "Red";
    public const string GreenImports = "GreenImports";

    public static readonly IReadOnlyList<string> All = [Blue, Yellow, Green, Red, GreenImports];

    public static bool IsValid(string? track) => track is not null && All.Contains(track);

    public static bool AllowsInspection(string? track) => track is Red or GreenImports;
}

// Customs Finance decision for an Open Ocean Import shipment (no row = never decided).
// Pending is set by Restore; Deferred returns to the request list on DeferredUntilDate.
public enum CustomsFinanceStatus
{
    Pending = 1,
    Approved = 2,
    Deferred = 3,
    OnHold = 4
}
