using Content.Application.Content.DeleteContent;
using MediatR;

namespace Api.Modules.Content.DeleteContent;

public static class DeleteContentEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapDelete(
            "/api/content/{id:guid}",
            async (
                Guid id,
                ISender sender,
                CancellationToken ct) =>
            {
                var command = new DeleteContentCommand(id);

                var result = await sender.Send(command, ct);

                return result.IsSuccess
                    ? Results.NoContent()
                    : Results.BadRequest(result.Error);
            });
    }
}