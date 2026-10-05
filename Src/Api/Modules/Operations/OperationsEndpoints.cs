using Api.Modules.Operations.CreateOperation;

namespace Api.Modules.Operations;

public static class OperationsEndpoints
{
    public static void MapOperationsEndpoints(
        this IEndpointRouteBuilder app)
    {
        CreateOperationEndpoint.Map(app);
    }
}