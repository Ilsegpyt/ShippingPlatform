namespace Reports.Domain.Report;

public sealed record ReportClassification(
    ReportCategory Category,
    ReportService Service,
    ReportShipmentType ShipmentType)
{
    public static bool IsValid(
        ReportCategory category,
        ReportService service,
        ReportShipmentType shipmentType)
    {
        return category switch
        {
            ReportCategory.Air =>
                service == ReportService.Freight &&
                shipmentType is
                    ReportShipmentType.Import or
                    ReportShipmentType.Export or
                    ReportShipmentType.All,

            ReportCategory.Sea =>
                service == ReportService.Freight &&
                shipmentType is
                    ReportShipmentType.Import or
                    ReportShipmentType.Export or
                    ReportShipmentType.All,

            ReportCategory.Domestic =>
                service is
                    ReportService.CustomsClearance or
                    ReportService.Transportation or
                    ReportService.Both &&
                shipmentType is
                    ReportShipmentType.Import or
                    ReportShipmentType.Export or
                    ReportShipmentType.All,

            ReportCategory.Financial =>
                service == ReportService.None &&
                shipmentType == ReportShipmentType.None,

            _ => false
        };
    }
}
