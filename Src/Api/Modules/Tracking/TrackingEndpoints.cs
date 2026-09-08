
namespace Api.Modules.Tracking;

public static class TrackingEndpoints
{
    public static IEndpointRouteBuilder MapTrackingEndpoints(
        this IEndpointRouteBuilder app)
    {
        GetShipmentTrackingEndpoint.Map(app);
        GetMyShipmentsTrackingEndpoint.Map(app);

        return app;
    }
}