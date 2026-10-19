using Api.Modules.Operations.ApproveFclSuggestion;
using Api.Modules.Operations.CreateOperation;
using Api.Modules.Operations.DeliverOperation;
using Api.Modules.Operations.GenerateFclSuggestions;
using Api.Modules.Operations.GetFclSuggestions;
using Api.Modules.Operations.ReopenOperation;
using Api.Modules.Operations.UpdateExport;
using Api.Modules.Operations.UpdateImport;

namespace Api.Modules.Operations;

public static class OperationsEndpoints
{
    public static void MapOperationsEndpoints(this IEndpointRouteBuilder app)
    {
        CreateOperationEndpoint.Map(app);
        UpdateImportEndpoint.Map(app);
        DeliverOperationEndpoint.Map(app);
        ReopenOperationEndpoint.Map(app);
        GenerateFclSuggestionsEndpoint.Map(app);
        GetFclSuggestionsEndpoint.Map(app);
        ApproveFclSuggestionEndpoint.Map(app);
        UpdateExportEndpoint.Map(app);
    }
}
