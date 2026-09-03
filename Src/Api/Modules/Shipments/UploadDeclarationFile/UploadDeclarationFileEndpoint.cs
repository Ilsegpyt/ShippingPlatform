using Identity.Domain;
using Identity.Infrastructure.Authorization;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Shipments.Application.Shipments.UploadDeclarationFile;
using System.Security.Claims;

namespace Api.Modules.Shipments.UploadDeclarationFile;

public static class UploadDeclarationFileEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPost(
            "/api/shipments/{shipmentId:guid}/declaration-files",
            async (
                Guid shipmentId,
                [FromForm] UploadDeclarationFileRequest request,
                ISender sender,
                ClaimsPrincipal user,
                CancellationToken ct) =>
            {
                var customerId = user.GetOrganizationId();

                var command = new UploadDeclarationFileCommand(
                    shipmentId,
                    customerId,
                    request.File.FileName,
                    request.File.OpenReadStream());

                var result = await sender.Send(command, ct);

                return result.IsSuccess
                    ? Results.Ok(result.Value)
                    : Results.BadRequest(result.Error);
            })
            .DisableAntiforgery()
            .RequirePermission(PermissionCatalog.ShipmentsBook);
    }
}