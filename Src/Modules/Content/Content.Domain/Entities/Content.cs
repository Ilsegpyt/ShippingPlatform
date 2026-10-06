namespace Content.Domain.Entities;

public sealed class Content
{
    public Guid Id { get; private set; }

    public Guid? ParentId { get; private set; }

    public string Title { get; private set; } = null!;

    public ContentType Type { get; private set; }

    public string? Body { get; private set; }

    public string? FeaturedImage { get; private set; }

    public string? LinkUrl { get; private set; }

    // Post-specific fields
    public string? Category { get; private set; }

    public DateTime? PublishedAt { get; private set; }

    public Content? Parent { get; private set; }

    public ICollection<Content> Children { get; private set; } = new List<Content>();

    public static Content Create(
        Guid? parentId,
        string title,
        ContentType type,
        string? body,
        string? featuredImage,
        string? linkUrl,
        string? category,
        DateTime? publishedAt)
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
            publishedAt);
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
        DateTime? publishedAt)
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
    }

    public void Update(
        Guid? parentId,
        string title,
        ContentType type,
        string? body,
        string? featuredImage,
        string? linkUrl,
        string? category,
        DateTime? publishedAt)
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
    }
}

public enum ContentType
{
    Category = 1,
    Page = 2,
    Post = 3,
    Link = 4
}