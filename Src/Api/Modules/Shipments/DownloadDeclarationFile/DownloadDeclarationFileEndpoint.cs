using Identity.Domain.ValueObjects;
using Identity.Infrastructure.Authorization;
using MediatR;
using Shipments.Application.Abstractions;
using Shipments.Application.Shipments.DownloadDeclarationFile;
using System.Security.Claims;

namespace Api.Modules.Shipments.DownloadDeclarationFile;

public static class DownloadDeclarationFileEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapGet(
            "/api/shipments/{shipmentId:guid}/declaration-files/{fileId:guid}/download",
            async (
                Guid shipmentId,
                Guid fileId,
                ClaimsPrincipal user,
                ISender sender,
                IFileStorage fileStorage,
                CancellationToken ct) =>
            {
                var tokenType = user.FindFirstValue("token_type");

                Guid? customerId = null;

                if (tokenType != "internal")
                {
                    customerId = user.GetOrganizationId();
                }

                var result = await sender.Send(
                    new DownloadDeclarationFileQuery(
                        shipmentId,
                        fileId,
                        customerId),
                    ct);

                if (result is null)
                {
                    return Results.NotFound();
                }

                var stream = await fileStorage.OpenReadAsync(
                    result.StorageKey,
                    ct);

                return Results.File(
                    stream,
                    GetContentType(result.FileName),
                    result.FileName);
            })
            .RequirePermission(PermissionCatalog.ShipmentsView);
    }

    private static string GetContentType(string fileName)
    {
        return Path.GetExtension(fileName).ToLowerInvariant() switch
        {
            ".pdf" =>
                "application/pdf",

            ".xlsx" =>
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",

            ".xls" =>
                "application/vnd.ms-excel",

            ".csv" =>
                "text/csv",

            ".docx" =>
                "application/vnd.openxmlformats-officedocument.wordprocessingml.document",

            ".jpg" or ".jpeg" =>
                "image/jpeg",

            ".png" =>
                "image/png",

            _ =>
                "application/octet-stream"
        };
    }
}