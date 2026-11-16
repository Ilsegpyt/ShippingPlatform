// Common Endpoints
using Api.Modules.Operations.Common.CreateDestination;
using Api.Modules.Operations.Common.CreateShippingLine;
using Api.Modules.Operations.Common.DeactivateDestination;
using Api.Modules.Operations.Common.DeactivateShippingLine;
using Api.Modules.Operations.Common.GetOperationDestinations;
using Api.Modules.Operations.Common.GetOperationShippingLines;
using Api.Modules.Operations.Common.UpdateDestination;
using Api.Modules.Operations.Common.UpdateShippingLine;

// FCL Endpoints
using Api.Modules.Operations.Fcl.AddContainerUpdate;
using Api.Modules.Operations.Fcl.ApproveFclSuggestion;
using Api.Modules.Operations.Fcl.CreateDriver;
using Api.Modules.Operations.Fcl.CreateOperation;
using Api.Modules.Operations.Fcl.CreatePort;
using Api.Modules.Operations.Fcl.CreateTransporter;
using Api.Modules.Operations.Fcl.CreateVehicle;
using Api.Modules.Operations.Fcl.DeactivateDriver;
using Api.Modules.Operations.Fcl.DeactivatePort;
using Api.Modules.Operations.Fcl.DeactivateTransporter;
using Api.Modules.Operations.Fcl.DeactivateVehicle;
using Api.Modules.Operations.Fcl.DeleteOperation;
using Api.Modules.Operations.Fcl.DeliverOperation;
using Api.Modules.Operations.Fcl.ExportOperationsExcel;
using Api.Modules.Operations.Fcl.GenerateFclSuggestions;
using Api.Modules.Operations.Fcl.GetAdvancedOperationsSearch;
using Api.Modules.Operations.Fcl.GetAllFclApprovals;
using Api.Modules.Operations.Fcl.GetAllFclSuggestions;
using Api.Modules.Operations.Fcl.GetAllOperations;
using Api.Modules.Operations.Fcl.GetDeletedOperations;
using Api.Modules.Operations.Fcl.GetDeliveredOperations;
using Api.Modules.Operations.Fcl.GetFclSuggestions;
using Api.Modules.Operations.Fcl.GetOperationById;
using Api.Modules.Operations.Fcl.GetOperationCustomers;
using Api.Modules.Operations.Fcl.GetOperationDrivers;
using Api.Modules.Operations.Fcl.GetOperationPorts;
using Api.Modules.Operations.Fcl.GetOperationsCounts;
using Api.Modules.Operations.Fcl.GetOperationTransporters;
using Api.Modules.Operations.Fcl.GetOperationVehicles;
using Api.Modules.Operations.Fcl.ImportTrips;
using Api.Modules.Operations.Fcl.LockOperation;
using Api.Modules.Operations.Fcl.ReopenOperation;
using Api.Modules.Operations.Fcl.TakeOverOperation;
using Api.Modules.Operations.Fcl.UnlockOperation;
using Api.Modules.Operations.Fcl.UpdateDriver;
using Api.Modules.Operations.Fcl.UpdateExport;
using Api.Modules.Operations.Fcl.UpdateImport;
using Api.Modules.Operations.Fcl.UpdatePort;
using Api.Modules.Operations.Fcl.UpdateTransporter;
using Api.Modules.Operations.Fcl.UpdateVehicle;

// FTL Airport Endpoints
using Api.Modules.Operations.Ftl.AddFtlMasterItem;
using Api.Modules.Operations.Ftl.AddFtlTruckUpdate;
using Api.Modules.Operations.Ftl.CreateFtlOperation;
using Api.Modules.Operations.Ftl.DeleteFtlOperation;
using Api.Modules.Operations.Ftl.GetAllFtlOperations;
using Api.Modules.Operations.Ftl.GetFtlMaster;
using Api.Modules.Operations.Ftl.GetFtlUserMe;
using Api.Modules.Operations.Ftl.ImportFtlTrips;
using Api.Modules.Operations.Ftl.LockFtlOperation;
using Api.Modules.Operations.Ftl.UnlockFtlOperation;
using Api.Modules.Operations.Ftl.UpdateFtlOperation;
using Api.Modules.Operations.Ftl.UpdateFtlStatus;

namespace Api.Modules.Operations;

public static class OperationsEndpoints
{
    public static void MapOperationsEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapCommonOperationsEndpoints();
        app.MapFclOperationsEndpoints();
        app.MapFtlOperationsEndpoints();
    }

    public static void MapCommonOperationsEndpoints(this IEndpointRouteBuilder app)
    {
        // Destinations
        CreateDestinationEndpoint.Map(app);
        UpdateDestinationEndpoint.Map(app);
        DeactivateDestinationEndpoint.Map(app);
        GetOperationDestinationsEndpoint.Map(app);

        // Shipping Lines
        CreateShippingLineEndpoint.Map(app);
        UpdateShippingLineEndpoint.Map(app);
        DeactivateShippingLineEndpoint.Map(app);
        GetOperationShippingLinesEndpoint.Map(app);
    }

    public static void MapFclOperationsEndpoints(this IEndpointRouteBuilder app)
    {
        CreateOperationEndpoint.Map(app);
        ImportTripsEndpoint.Map(app);
        GetOperationCustomersEndpoint.Map(app);
        GetAllOperationsEndpoint.Map(app);
        GetOperationByIdEndpoint.Map(app);
        GetDeliveredOperationsEndpoint.Map(app);
        GetAdvancedOperationsSearchEndpoint.Map(app);
        ExportOperationsExcelEndpoint.Map(app);
        UpdateImportEndpoint.Map(app);
        DeliverOperationEndpoint.Map(app);
        ReopenOperationEndpoint.Map(app);
        DeleteOperationEndpoint.Map(app);
        GetDeletedOperationsEndpoint.Map(app);
        GetOperationsCountsEndpoint.Map(app);
        GenerateFclSuggestionsEndpoint.Map(app);
        GetFclSuggestionsEndpoint.Map(app);
        ApproveFclSuggestionEndpoint.Map(app);
        UpdateExportEndpoint.Map(app);
        GetAllFclSuggestionsEndpoint.Map(app);
        GetAllFclApprovalsEndpoint.Map(app);
        AddContainerUpdateEndpoint.Map(app);
        LockOperationEndpoint.Map(app);
        UnlockOperationEndpoint.Map(app);
        TakeOverOperationEndpoint.Map(app);

        // FCL Master Data (Ports, Drivers, Transporters, Vehicles)
        CreatePortEndpoint.Map(app);
        UpdatePortEndpoint.Map(app);
        DeactivatePortEndpoint.Map(app);
        GetOperationPortsEndpoint.Map(app);

        CreateDriverEndpoint.Map(app);
        UpdateDriverEndpoint.Map(app);
        DeactivateDriverEndpoint.Map(app);
        GetOperationDriversEndpoint.Map(app);

        CreateTransporterEndpoint.Map(app);
        UpdateTransporterEndpoint.Map(app);
        DeactivateTransporterEndpoint.Map(app);
        GetOperationTransportersEndpoint.Map(app);

        CreateVehicleEndpoint.Map(app);
        UpdateVehicleEndpoint.Map(app);
        DeactivateVehicleEndpoint.Map(app);
        GetOperationVehiclesEndpoint.Map(app);
    }

    public static void MapFtlOperationsEndpoints(this IEndpointRouteBuilder app)
    {
        CreateFtlOperationEndpoint.Map(app);
        GetAllFtlOperationsEndpoint.Map(app);
        UpdateFtlOperationEndpoint.Map(app);
        UpdateFtlStatusEndpoint.Map(app);
        DeleteFtlOperationEndpoint.Map(app);
        LockFtlOperationEndpoint.Map(app);
        UnlockFtlOperationEndpoint.Map(app);
        AddFtlTruckUpdateEndpoint.Map(app);
        ImportFtlTripsEndpoint.Map(app);
        GetFtlMasterEndpoint.Map(app);
        AddFtlMasterItemEndpoint.Map(app);
        GetFtlUserMeEndpoint.Map(app);
    }
}
