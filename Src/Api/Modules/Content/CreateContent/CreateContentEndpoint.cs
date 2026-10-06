using Content.Application.Content.CreateContent;
using Content.Domain.Entities;
using Identity.Domain.ValueObjects;
using Identity.Infrastructure.Authorization;
using MediatR;

namespace Api.Modules.Content.CreateContent;

public static class CreateContentEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/content", async (
            CreateContentRequest request,
            ISender sender,
            CancellationToken ct) =>
        {
            var command = new CreateContentCommand(
                request.ParentId,
                request.Title,
                request.Type,
                request.Body,
                request.FeaturedImage,
                request.LinkUrl,
                request.Category,
                request.PublishedAt);

            var result = await sender.Send(command, ct);

            return result.IsSuccess
                ? Results.Ok(result.Value)
                : Results.BadRequest(result.Error);
        })
            .RequirePermission(PermissionCatalog.ContentCreate);
    }
}

public sealed record CreateContentRequest(
    Guid? ParentId,
    string Title,
    ContentType Type,
    string? Body,
    string? FeaturedImage,
    string? LinkUrl,
    string? Category,
    DateTime? PublishedAt);