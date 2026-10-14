
using System.Text.Json;
using FluentValidation;
using Content.Domain.Entities;

namespace Content.Application.Content.CreateContent;

public sealed class CreateContentCommandValidator
    : AbstractValidator<CreateContentCommand>
{
    private static readonly JsonDocumentOptions JsonOptions = new()
    {
        MaxDepth = 32
    };

    public CreateContentCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Type)
            .IsInEnum();

        RuleFor(x => x.Body)
            .MaximumLength(100000);

        RuleFor(x => x.FeaturedImage)
            .MaximumLength(1000);

        RuleFor(x => x.LinkUrl)
            .MaximumLength(2000);

        RuleFor(x => x.Category)
            .MaximumLength(100);

        RuleFor(x => x.PageIntroText)
            .MaximumLength(100000);

        RuleFor(x => x.PageContentOrder)
            .MaximumLength(1000000);

        // Category
        When(x => x.Type == ContentType.Category, () =>
        {
            RuleFor(x => x.Body)
                .Empty()
                .WithMessage("Category content cannot have a body.");

            RuleFor(x => x.FeaturedImage)
                .Empty()
                .WithMessage("Category content cannot have a featured image.");

            RuleFor(x => x.LinkUrl)
                .Empty()
                .WithMessage("Category content cannot have a link URL.");

            RuleFor(x => x.Category)
                .Empty()
                .WithMessage("Category content cannot have a category.");

            RuleFor(x => x.PublishedAt)
                .Null()
                .WithMessage("Category content cannot have a published date.");

            RuleFor(x => x.PageIntroText)
                .Empty()
                .WithMessage("Category content cannot have page intro text.");

            RuleFor(x => x.PageContentOrder)
                .Empty()
                .WithMessage("Category content cannot have page content order.");
        });

        // Page
        When(x => x.Type == ContentType.Page, () =>
        {
            RuleFor(x => x.Body)
                .Empty()
                .WithMessage("Page content cannot have a body.");

            RuleFor(x => x.PageContentOrder)
                .NotEmpty()
                .WithMessage("Page content order is required.")
                .Must(HasValidPageStructure)
                .WithMessage(
                    "Page content order must contain at least one Section, every Section must have a title, and all content types must be supported.");

            RuleFor(x => x.FeaturedImage)
                .Empty()
                .WithMessage("Page content cannot have a featured image.");

            RuleFor(x => x.LinkUrl)
                .Empty()
                .WithMessage("Page content cannot have a link URL.");

            RuleFor(x => x.Category)
                .Empty()
                .WithMessage("Page content cannot have a category.");

            RuleFor(x => x.PublishedAt)
                .Null()
                .WithMessage("Page content cannot have a published date.");

            // PageIntroText is optional.
        });

        // Post
        When(x => x.Type == ContentType.Post, () =>
        {
            RuleFor(x => x.Body)
                .NotEmpty()
                .WithMessage("Body is required for Post content.");

            RuleFor(x => x.FeaturedImage)
                .NotEmpty()
                .WithMessage("Featured image is required for Post content.");

            RuleFor(x => x.LinkUrl)
                .Empty()
                .WithMessage("Post content cannot have a link URL.");

            RuleFor(x => x.Category)
                .NotEmpty()
                .WithMessage("Category is required for Post content.");

            RuleFor(x => x.PublishedAt)
                .NotNull()
                .WithMessage("Published date is required for Post content.");

            RuleFor(x => x.PageIntroText)
                .Empty()
                .WithMessage("Post content cannot have page intro text.");

            RuleFor(x => x.PageContentOrder)
                .Empty()
                .WithMessage("Post content cannot have page content order.");
        });

        // Link
        When(x => x.Type == ContentType.Link, () =>
        {
            RuleFor(x => x.Body)
                .Empty()
                .WithMessage("Link content cannot have a body.");

            RuleFor(x => x.FeaturedImage)
                .Empty()
                .WithMessage("Link content cannot have a featured image.");

            RuleFor(x => x.LinkUrl)
                .NotEmpty()
                .WithMessage("Link URL is required for Link content.");

            RuleFor(x => x.Category)
                .Empty()
                .WithMessage("Link content cannot have a category.");

            RuleFor(x => x.PublishedAt)
                .Null()
                .WithMessage("Link content cannot have a published date.");

            RuleFor(x => x.PageIntroText)
                .Empty()
                .WithMessage("Link content cannot have page intro text.");

            RuleFor(x => x.PageContentOrder)
                .Empty()
                .WithMessage("Link content cannot have page content order.");
        });
    }

    private static bool HasValidPageStructure(string? json)
    {
        if (string.IsNullOrWhiteSpace(json))
            return false;

        try
        {
            using var document = JsonDocument.Parse(json, JsonOptions);

            var root = document.RootElement;

            if (root.ValueKind != JsonValueKind.Object)
                return false;

            if (!root.TryGetProperty("version", out var version)
                || version.ValueKind != JsonValueKind.Number
                || !version.TryGetInt32(out var versionNumber)
                || versionNumber != 1)
            {
                return false;
            }

            if (!root.TryGetProperty("items", out var items)
                || items.ValueKind != JsonValueKind.Array)
            {
                return false;
            }

            var sectionCount = 0;

            foreach (var item in items.EnumerateArray())
            {
                if (item.ValueKind != JsonValueKind.Object)
                    return false;

                if (!HasValidId(item))
                    return false;

                if (!TryGetString(item, "type", out var type))
                    return false;

                switch (type)
                {
                    case "Section":
                        sectionCount++;


                        if (!TryGetString(item, "title", out var sectionTitle)
                            || string.IsNullOrWhiteSpace(sectionTitle))
                        {
                            return false;
                        }

                        if (!HasValidChildren(
                                item,
                                "items",
                                "CardText",
                                "HighlightText",
                                "ClosingText"))
                        {
                            return false;
                        }

                        break;

                    case "PageHighlights":
                        if (!HasValidChildren(
                                item,
                                "items",
                                "HighlightText"))
                        {
                            return false;
                        }

                        break;

                    case "PageClosings":
                        if (!HasValidChildren(
                                item,
                                "items",
                                "ClosingText"))
                        {
                            return false;
                        }

                        break;

                    default:
                        return false;
                }
            }

            return sectionCount >= 1;
        }
        catch (JsonException)
        {
            return false;
        }
    }

    private static bool HasValidChildren(
        JsonElement parent,
        string propertyName,
        params string[] allowedTypes)
    {
        if (!parent.TryGetProperty(propertyName, out var children)
            || children.ValueKind != JsonValueKind.Array)
        {
            return false;
        }

        foreach (var child in children.EnumerateArray())
        {
            if (child.ValueKind != JsonValueKind.Object)
                return false;

            if (!HasValidId(child))
                return false;

            if (!TryGetString(child, "type", out var type)
                || !allowedTypes.Contains(type))
            {
                return false;
            }

            if (!child.TryGetProperty("text", out var text)
                || text.ValueKind != JsonValueKind.String)
            {
                return false;
            }
        }

        return true;
    }

    private static bool HasValidId(JsonElement element)
    {
        return TryGetString(element, "id", out var id)
            && !string.IsNullOrWhiteSpace(id);
    }

    private static bool TryGetString(
        JsonElement element,
        string propertyName,
        out string value)
    {
        value = string.Empty;

        if (!element.TryGetProperty(propertyName, out var property)
            || property.ValueKind != JsonValueKind.String)
        {
            return false;
        }

        value = property.GetString() ?? string.Empty;

        return true;
    }
}
