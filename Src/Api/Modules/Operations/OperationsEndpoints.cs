using Api.Modules.Operations.AddContainerUpdate;
using Api.Modules.Operations.ExportOperationsExcel;
using Api.Modules.Operations.ApproveFclSuggestion;
using Api.Modules.Operations.CreateOperation;
using Api.Modules.Operations.DeliverOperation;
using Api.Modules.Operations.GenerateFclSuggestions;
using Api.Modules.Operations.GetAllFclApprovals;
using Api.Modules.Operations.GetAllFclSuggestions;
using Api.Modules.Operations.GetAllOperations;
using Api.Modules.Operations.GetFclSuggestions;
using Api.Modules.Operations.GetOperationById;
using Api.Modules.Operations.GetDeliveredOperations;
using Api.Modules.Operations.GetAdvancedOperationsSearch;
using Api.Modules.Operations.ReopenOperation;
using Api.Modules.Operations.UpdateExport;
using Api.Modules.Operations.UpdateImport;
using Api.Modules.Operations.ImportTrips;
using Api.Modules.Operations.GetOperationCustomers;
using Api.Modules.Operations.GetOperationShippingLines;
using Api.Modules.Operations.GetOperationPorts;
using Api.Modules.Operations.GetOperationTransporters;
using Api.Modules.Operations.GetOperationDrivers;
using Api.Modules.Operations.GetOperationVehicles;
using Api.Modules.Operations.GetOperationDestinations;
using Api.Modules.Operations.CreateDestination;
using Api.Modules.Operations.DeleteOperation;
using Api.Modules.Operations.GetDeletedOperations;
using Api.Modules.Operations.GetOperationsCounts;

// Lookups Master Data Endpoints
using Api.Modules.Operations.CreatePort;
using Api.Modules.Operations.CreateDriver;
using Api.Modules.Operations.CreateShippingLine;
using Api.Modules.Operations.CreateTransporter;
using Api.Modules.Operations.CreateVehicle;

using Api.Modules.Operations.UpdatePort;
using Api.Modules.Operations.UpdateDestination;
using Api.Modules.Operations.UpdateDriver;
using Api.Modules.Operations.UpdateShippingLine;
using Api.Modules.Operations.UpdateTransporter;
using Api.Modules.Operations.UpdateVehicle;

using Api.Modules.Operations.DeactivatePort;
using Api.Modules.Operations.DeactivateDestination;
using Api.Modules.Operations.DeactivateDriver;
using Api.Modules.Operations.DeactivateShippingLine;
using Api.Modules.Operations.DeactivateTransporter;
using Api.Modules.Operations.DeactivateVehicle;

namespace Api.Modules.Operations;

public static class OperationsEndpoints
{
    public static void MapOperationsEndpoints(this IEndpointRouteBuilder app)
    {
        CreateOperationEndpoint.Map(app);
        ImportTripsEndpoint.Map(app);
        GetOperationCustomersEndpoint.Map(app);
        GetOperationShippingLinesEndpoint.Map(app);
        GetOperationPortsEndpoint.Map(app);
        GetOperationTransportersEndpoint.Map(app);
        GetOperationDriversEndpoint.Map(app);
        GetOperationVehiclesEndpoint.Map(app);
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
        GetOperationDestinationsEndpoint.Map(app);
        CreateDestinationEndpoint.Map(app);

        // Lookups Master Data (Add, Edit, Inactive)
        CreatePortEndpoint.Map(app);
        CreateDriverEndpoint.Map(app);
        CreateShippingLineEndpoint.Map(app);
        CreateTransporterEndpoint.Map(app);
        CreateVehicleEndpoint.Map(app);

        UpdatePortEndpoint.Map(app);
        UpdateDestinationEndpoint.Map(app);
        UpdateDriverEndpoint.Map(app);
        UpdateShippingLineEndpoint.Map(app);
        UpdateTransporterEndpoint.Map(app);
        UpdateVehicleEndpoint.Map(app);

        DeactivatePortEndpoint.Map(app);
        DeactivateDestinationEndpoint.Map(app);
        DeactivateDriverEndpoint.Map(app);
        DeactivateShippingLineEndpoint.Map(app);
        DeactivateTransporterEndpoint.Map(app);
        DeactivateVehicleEndpoint.Map(app);
    }
}
