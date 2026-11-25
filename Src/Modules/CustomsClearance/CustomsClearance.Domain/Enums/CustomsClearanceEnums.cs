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
