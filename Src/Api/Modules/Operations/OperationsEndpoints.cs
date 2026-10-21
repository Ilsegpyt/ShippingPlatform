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
        GenerateFclSuggestionsEndpoint.Map(app);
        GetFclSuggestionsEndpoint.Map(app);
        ApproveFclSuggestionEndpoint.Map(app);
        UpdateExportEndpoint.Map(app);
        GetAllFclSuggestionsEndpoint.Map(app);
        GetAllFclApprovalsEndpoint.Map(app);
    }
}
