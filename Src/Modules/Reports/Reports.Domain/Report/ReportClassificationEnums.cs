namespace Reports.Domain.Report;

public enum ReportCategory
{
    Sea = 1,
    Air = 2,
    Domestic = 3,
    Financial = 4
}

public enum ReportService
{
    None = 0,
    Freight = 1,
    CustomsClearance = 2,
    Transportation = 3,
    Both = 4
}

public enum ReportShipmentType
{
    None = 0,
    All = 1,
    Import = 2,
    Export = 3
}