
namespace Content.Domain.Entities;

public sealed class Content
{
    public Guid Id { get; private set; }

    public Guid? ParentId { get; private set; }

    public string Title { get; private set; } = null!;

    public ContentType Type { get; private set; }

    // Existing content field. Preserved for current content types.
    public string? Body { get; private set; }

    public string? FeaturedImage { get; private set; }

    public string? LinkUrl { get; private set; }

    // Post-specific fields
    public string? Category { get; private set; }

    public DateTime? PublishedAt { get; private set; }

    // Page-specific fields
    public string? PageIntroText { get; private set; }

    // Ordered page structure serialized as JSON.
    public string? PageContentOrder { get; private set; }

    public Content? Parent { get; private set; }

    public ICollection<Content> Children { get; private set; }
        = new List<Content>();

    public static Content Create(
        Guid? parentId,
        string title,
        ContentType type,
        string? body,
        string? featuredImage,
        string? linkUrl,
        string? category,
        DateTime? publishedAt,
        string? pageIntroText = null,
        string? pageContentOrder = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(title);

        return new Content(
            Guid.NewGuid(),
            parentId,
            title,
            type,
            body,
            featuredImage,
            linkUrl,
            category,
            publishedAt,
            pageIntroText,
            pageContentOrder);
    }

    private Content()
    {
    }

    private Content(
        Guid id,
        Guid? parentId,
        string title,
        ContentType type,
        string? body,
        string? featuredImage,
        string? linkUrl,
        string? category,
        DateTime? publishedAt,
        string? pageIntroText,
        string? pageContentOrder)
    {
        Id = id;
        ParentId = parentId;
        Title = title;
        Type = type;
        Body = body;
        FeaturedImage = featuredImage;
        LinkUrl = linkUrl;
        Category = category;
        PublishedAt = publishedAt;
        PageIntroText = pageIntroText;
        PageContentOrder = pageContentOrder;
    }

    public void Update(
        Guid? parentId,
        string title,
        ContentType type,
        string? body,
        string? featuredImage,
        string? linkUrl,
        string? category,
        DateTime? publishedAt,
        string? pageIntroText = null,
        string? pageContentOrder = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(title);

        ParentId = parentId;
        Title = title;
        Type = type;
        Body = body;
        FeaturedImage = featuredImage;
        LinkUrl = linkUrl;
        Category = category;
        PublishedAt = publishedAt;
        PageIntroText = pageIntroText;
        PageContentOrder = pageContentOrder;
    }
}

public enum ContentType
{
    Category = 1,
    Page = 2,
    Post = 3,
    Link = 4
}
