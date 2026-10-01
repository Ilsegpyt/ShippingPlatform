using Content.Application.Content.UpdateContent;
using Content.Domain.Entities;
using Identity.Domain.ValueObjects;
using Identity.Infrastructure.Authorization;
using MediatR;

namespace Api.Modules.Content.UpdateContent;

public static class UpdateContentEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPut("/api/content/{id:guid}", async (
            Guid id,
            UpdateContentRequest request,
            ISender sender,
            CancellationToken ct) =>
        {
            var command = new UpdateContentCommand(
                id,
                request.ParentId,
                request.Title,
                request.Type,
                request.Body,
                request.FeaturedImage,
                request.LinkUrl);

            var result = await sender.Send(command, ct);

            return result.IsSuccess
                ? Results.NoContent()
                : Results.BadRequest(result.Error);
        })
            .RequirePermission(PermissionCatalog.ContentEdit);
    }
}

public sealed record UpdateContentRequest(
    Guid? ParentId,
    string Title,
    ContentType Type,
    string? Body,
    string? FeaturedImage,
    string? LinkUrl);