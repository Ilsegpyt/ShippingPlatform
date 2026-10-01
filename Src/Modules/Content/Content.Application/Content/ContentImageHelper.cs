using System.Text.RegularExpressions;

namespace Content.Application.Content;

public static class ContentImageHelper
{
    // Extracts local image URLs from the featured image and HTML body.
    public static HashSet<string> ExtractLocalImageUrls(
        string? featuredImage,
        string? body)
    {
        var imageUrls = new HashSet<string>(
            StringComparer.OrdinalIgnoreCase);

        // Include the featured image if it belongs to our uploads folder.
        AddLocalImageUrl(imageUrls, featuredImage);

        // Extract image sources from the HTML body.
        if (!string.IsNullOrWhiteSpace(body))
        {
            var matches = Regex.Matches(
                body,
                """src\s*=\s*["']([^"']+)["']""",
                RegexOptions.IgnoreCase);

            foreach (Match match in matches)
            {
                AddLocalImageUrl(
                    imageUrls,
                    match.Groups[1].Value);
            }
        }

        return imageUrls;
    }

    // Accepts only images stored directly inside /uploads/content/.
    private static void AddLocalImageUrl(
        HashSet<string> imageUrls,
        string? imageUrl)
    {
        if (string.IsNullOrWhiteSpace(imageUrl))
            return;

        var value = imageUrl.Trim();

        // Convert absolute URLs into paths.
        if (Uri.TryCreate(value, UriKind.Absolute, out var uri))
        {
            value = uri.AbsolutePath;
        }

        // Remove query strings.
        var queryIndex = value.IndexOf('?');

        if (queryIndex >= 0)
            value = value[..queryIndex];

        value = Uri.UnescapeDataString(value)
            .Replace('\\', '/');

        const string allowedPrefix = "/uploads/content/";

        // Ignore external images and paths outside our uploads folder.
        if (!value.StartsWith(
                allowedPrefix,
                StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        // Reject nested paths and invalid file names.
        var fileName = value[allowedPrefix.Length..];

        if (string.IsNullOrWhiteSpace(fileName)
            || fileName.Contains('/')
            || fileName is "." or "..")
        {
            return;
        }

        imageUrls.Add(allowedPrefix + fileName);
    }
}