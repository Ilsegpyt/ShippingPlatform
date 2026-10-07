using MediatR;
using Website.Application.Branches.UpdateBranch;

namespace Api.Modules.Website.Branches.UpdateBranch;

public static class UpdateBranchEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPut(
            "/api/website/branches/{id:int}",
            async (
                int id,
                UpdateBranchRequest request,
                ISender sender,
                CancellationToken ct) =>
            {
                var command = new UpdateBranchCommand(
                    id,
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
            });
    }
}

public sealed record UpdateBranchRequest(
    string Name,
    string Address,
    decimal Latitude,
    decimal Longitude,
    string? CeoName,
    string? CeoEmail,
    string? GmName,
    string? GmEmail,
    string? BranchManagerName,
    string? BranchManagerEmail,
    bool IsActive);