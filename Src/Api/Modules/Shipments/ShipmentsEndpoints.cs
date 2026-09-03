using Api.Modules.Shipments.CreateShipment;
using Api.Modules.Shipments.DeleteDeclarationFiles;
using Api.Modules.Shipments.DeleteShipments;
using Api.Modules.Shipments.UpdateShipment;
using Api.Modules.Shipments.UploadDeclarationFile;

namespace Api.Modules.Shipments;

public static class ShipmentsEndpoints
{
    public static void MapShipmentsEndpoints(
        this IEndpointRouteBuilder app)
    {
        CreateShipmentEndpoint.Map(app);
        UpdateShipmentEndpoint.Map(app);
        UploadDeclarationFileEndpoint.Map(app);
        DeleteDeclarationFilesEndpoint.Map(app);
        DeleteShipmentsEndpoint.Map(app);
    }
}