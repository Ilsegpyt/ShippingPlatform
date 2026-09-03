using Api.Modules.Shipments.DeleteDeclarationFiles.DeleteDeclarationFiles;
using Identity.Domain;
using Identity.Infrastructure.Authorization;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Shipments.Application.Shipments.DeleteDeclarationFiles;
using System.Security.Claims;

namespace Api.Modules.Shipments.DeleteDeclarationFiles;

public static class DeleteDeclarationFilesEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapDelete(
            "/api/shipments/{shipmentId:guid}/declaration-files",
            async (
                Guid shipmentId,
                [FromBody] DeleteDeclarationFilesRequest request,
                ISender sender,
                ClaimsPrincipal user,
                CancellationToken ct) =>
            {
                var customerId = user.GetOrganizationId();

                var command = new DeleteDeclarationFilesCommand(
                    shipmentId,
                    customerId,
                    request.DeclarationFileIds);

                var result = await sender.Send(command, ct);

                return result.IsSuccess
                    ? Results.Ok(result.Value)
                    : Results.BadRequest(result.Error);
            })
            .RequirePermission(PermissionCatalog.ShipmentsBook);
    }
}