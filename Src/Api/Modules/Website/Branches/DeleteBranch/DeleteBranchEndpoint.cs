using Identity.Domain.ValueObjects;
using Identity.Infrastructure.Authorization;
using MediatR;
using Website.Application.Branches.DeleteBranch;

namespace Api.Modules.Website.Branches.DeleteBranch;

public static class DeleteBranchEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapDelete(
            "/api/website/branches/{id:int}",
            async (
                int id,
                ISender sender,
                CancellationToken ct) =>
            {
                var command = new DeleteBranchCommand(id);

                var result = await sender.Send(command, ct);

                return result.IsSuccess
                    ? Results.Ok()
                    : Results.BadRequest(result.Error);
            }).RequirePermission(PermissionCatalog.BranchesDelete);
    }
}