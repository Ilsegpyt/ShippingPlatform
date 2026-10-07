using MediatR;
using Microsoft.AspNetCore.StaticFiles;
using Website.Application.RecruitmentApplications.DownloadRecruitmentApplicationCv;

namespace Api.Modules.Website.RecruitmentApplications.DownloadRecruitmentApplicationCv;

public static class DownloadRecruitmentApplicationCvEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapGet(
            "/api/website/recruitment-applications/{id:int}/cv",
            async (
                int id,
                ISender sender,
                IWebHostEnvironment environment,
                CancellationToken ct) =>
            {
                var query =
                    new DownloadRecruitmentApplicationCvQuery(id);

                var result = await sender.Send(query, ct);

                if (!result.IsSuccess)
                {
                    return Results.NotFound(result.Error);
                }

                var application = result.Value;

                if (string.IsNullOrWhiteSpace(
                        application.CvFilePath))
                {
                    return Results.NotFound(
                        "CV file is not available.");
                }

                var filePath = ResolveFilePath(
                    application.CvFilePath,
                    environment);

                if (filePath is null)
                {
                    return Results.NotFound(
                        "CV file was not found.");
                }

                if (!File.Exists(filePath))
                {
                    return Results.NotFound(
                        "CV file was not found.");
                }

                var fileBytes =
                    await File.ReadAllBytesAsync(
                        filePath,
                        ct);

                if (fileBytes.Length == 0)
                {
                    return Results.NotFound(
                        "CV file is empty.");
                }

                var provider =
                    new FileExtensionContentTypeProvider();

                string contentType;

                if (!string.IsNullOrWhiteSpace(
                        application.CvFileName) &&
                    Path.GetExtension(
                        application.CvFileName)
                        .Equals(
                            ".pdf",
                            StringComparison.OrdinalIgnoreCase))
                {
                    contentType = "application/pdf";
                }
                else if (
                    !string.IsNullOrWhiteSpace(
                        application.CvFileName) &&
                    provider.TryGetContentType(
                        application.CvFileName,
                        out var detectedContentType))
                {
                    contentType = detectedContentType;
                }
                else
                {
                    contentType =
                        "application/octet-stream";
                }

                return Results.File(
                    fileBytes,
                    contentType,
                    application.CvFileName);
            });
    }

    private static string? ResolveFilePath(
        string storedPath,
        IWebHostEnvironment environment)
    {
        if (string.IsNullOrWhiteSpace(storedPath))
        {
            return null;
        }

        var normalizedPath = storedPath
            .Trim()
            .Replace(
                '/',
                Path.DirectorySeparatorChar)
            .Replace(
                '\\',
                Path.DirectorySeparatorChar);

        string fullPath;

        if (Path.IsPathRooted(normalizedPath))
        {
            fullPath = Path.GetFullPath(
                normalizedPath);
        }
        else
        {
            var relativePath = normalizedPath
                .TrimStart(
                    Path.DirectorySeparatorChar);

            fullPath = Path.GetFullPath(
                Path.Combine(
                    environment.ContentRootPath,
                    relativePath));
        }

        if (IsInsideDirectory(
                fullPath,
                environment.ContentRootPath))
        {
            return fullPath;
        }

        if (!string.IsNullOrWhiteSpace(
                environment.WebRootPath) &&
            IsInsideDirectory(
                fullPath,
                environment.WebRootPath))
        {
            return fullPath;
        }

        return null;
    }

    private static bool IsInsideDirectory(
        string filePath,
        string directoryPath)
    {
        var fullFilePath =
            Path.GetFullPath(filePath)
                .TrimEnd(
                    Path.DirectorySeparatorChar)
            + Path.DirectorySeparatorChar;

        var fullDirectoryPath =
            Path.GetFullPath(directoryPath)
                .TrimEnd(
                    Path.DirectorySeparatorChar)
            + Path.DirectorySeparatorChar;

        return fullFilePath.StartsWith(
            fullDirectoryPath,
            StringComparison.OrdinalIgnoreCase);
    }
}