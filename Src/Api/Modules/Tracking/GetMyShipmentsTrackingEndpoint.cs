using Identity.Infrastructure.Authorization;
using MediatR;
using System.Security.Claims;
using Tracking.Application.Tracking.GetMyShipmentsTracking;

namespace Api.Modules.Tracking;

public static class GetMyShipmentsTrackingEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapGet(
            "/api/tracking/my-shipments",
            async (
                ClaimsPrincipal user,
                ISender sender,
                CancellationToken ct) =>
            {
                var customerId = user.GetOrganizationId();

                var result = await sender.Send(
                    new GetMyShipmentsTrackingQuery(customerId),
                    ct);

                return result.IsSuccess
                    ? Results.Ok(result.Value)
                    : Results.BadRequest(result.Error);
            });
    }
}