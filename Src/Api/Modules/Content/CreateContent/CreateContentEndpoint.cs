using Content.Application.Content.CreateContent;
using Content.Domain.Entities;
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
                request.LinkUrl);

            var result = await sender.Send(command, ct);

            return result.IsSuccess
                ? Results.Ok(result.Value)
                : Results.BadRequest(result.Error);
        });
    }
}

public sealed record CreateContentRequest(
    Guid? ParentId,
    string Title,
    ContentType Type,
    string? Body,
    string? FeaturedImage,
    string? LinkUrl);