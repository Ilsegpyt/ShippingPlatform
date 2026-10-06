using Api.Modules.Operations.CreateOperation;
using Api.Modules.Operations.UpdateImport;

namespace Api.Modules.Operations;

public static class OperationsEndpoints
{
    public static void MapOperationsEndpoints(
        this IEndpointRouteBuilder app)
    {
        CreateOperationEndpoint.Map(app);
        UpdateImportEndpoint.Map(app);
    }
}