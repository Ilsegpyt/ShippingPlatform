using Identity.Domain.ValueObjects;
using Identity.Infrastructure.Authorization;
using MediatR;
using Website.Application.Branches.CreateBranch;

namespace Api.Modules.Website.Branches.CreateBranch;

public static class CreateBranchEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPost(
            "/api/website/branches",
            async (
                CreateBranchRequest request,
                ISender sender,
                CancellationToken ct) =>
            {
                var command = new CreateBranchCommand(
                    request.Name,
                    request.Address,
                    request.Latitude,
                    request.Longitude,
                    request.CeoName,
                    request.CeoEmail,
                    request.GmName,
                    request.GmEmail,
                    request.BranchManagerName,
                    request.BranchManagerEmail,
                    request.IsActive);

                var result = await sender.Send(command, ct);

                return result.IsSuccess
                    ? Results.Ok()
                    : Results.BadRequest(result.Error);
            }).RequirePermission(PermissionCatalog.BranchesCreate);
    }
}

public sealed record CreateBranchRequest(
    string Name,
    string Address,
    decimal Latitude,
    decimal Longitude,
    string CeoName,
    string CeoEmail,
    string GmName,
    string GmEmail,
    string BranchManagerName,
    string BranchManagerEmail,
    bool IsActive);