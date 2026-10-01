using Content.Application.Abstractions;
using Identity.Domain.ValueObjects;
using Identity.Infrastructure.Authorization;

namespace Api.Modules.Content.UploadContentImage;

public static class UploadContentImageEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPost(
            "/api/content/upload-image",
            async (
                IFormFile file,
                IContentFileStorage fileStorage,
                CancellationToken ct) =>
            {
                if (file.Length == 0)
                {
                    return Results.BadRequest(
                        new { message = "File is empty." });
                }

                var url = await fileStorage.SaveAsync(
                    file.OpenReadStream(),
                    file.FileName,
                    ct);

                return Results.Ok(new
                {
                    url
                });
            })
             .DisableAntiforgery()
             .RequirePermission(PermissionCatalog.ContentEdit);
    }
}