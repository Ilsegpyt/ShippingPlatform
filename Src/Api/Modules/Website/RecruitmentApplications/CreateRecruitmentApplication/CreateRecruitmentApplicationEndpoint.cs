using Identity.Domain.ValueObjects;
using Identity.Infrastructure.Authorization;
using MediatR;
using Microsoft.AspNetCore.Http;
using Website.Application.RecruitmentApplications.CreateRecruitmentApplication;

namespace Api.Modules.Website.RecruitmentApplications.CreateRecruitmentApplication;

public static class CreateRecruitmentApplicationEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPost(
            "/api/website/recruitment-applications",
            async (
                HttpRequest httpRequest,
                ISender sender,
                IWebHostEnvironment environment,
                CancellationToken ct) =>
            {
                var form = await httpRequest.ReadFormAsync(ct);

                var department = form["department"].ToString();
                var message = form["message"].ToString();
                var cv = form.Files["cv"];

                if (cv is null || cv.Length == 0)
                    return Results.BadRequest("CV is required.");

                await using var signatureStream = cv.OpenReadStream();

                var signature = new byte[5];

                var bytesRead = await signatureStream.ReadAsync(
                    signature,
                    ct);

                if (bytesRead < 5 ||
                    signature[0] != 0x25 || // %
                    signature[1] != 0x50 || // P
                    signature[2] != 0x44 || // D
                    signature[3] != 0x46 || // F
                    signature[4] != 0x2D)   // -
                {
                    return Results.BadRequest(
                        "CV must be a valid PDF file.");
                }

                var uploadsPath = Path.Combine(
                    environment.ContentRootPath,
                    "Uploads",
                    "Recruitment");

                Directory.CreateDirectory(uploadsPath);

                var storedFileName =
                    $"{Guid.NewGuid():N}.pdf";

                var filePath = Path.Combine(
                    uploadsPath,
                    storedFileName);

                await using (var stream = new FileStream(
                    filePath,
                    FileMode.Create))
                {
                    await cv.CopyToAsync(stream, ct);
                }

                var command = new CreateRecruitmentApplicationCommand(
                    department,
                    cv.FileName,
                    filePath,
                    message);

                var result = await sender.Send(command, ct);

                return result.IsSuccess
                    ? Results.Ok()
                    : Results.BadRequest(result.Error);
            });
    }
}