using Identity.Domain.ValueObjects;
using Identity.Infrastructure.Authorization;
using MediatR;
using Website.Application.Branches.RestoreBranch;

namespace Api.Modules.Website.Branches.RestoreBranch;

public static class RestoreBranchEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPatch(
            "/api/website/branches/{id:int}/restore",
            async (
                int id,
                ISender sender,
                CancellationToken ct) =>
            {
                var command = new RestoreBranchCommand(id);

                var result = await sender.Send(command, ct);

                return result.IsSuccess
                    ? Results.Ok()
                    : Results.BadRequest(result.Error);
            }).RequirePermission(PermissionCatalog.BranchesRestore);
    }
}