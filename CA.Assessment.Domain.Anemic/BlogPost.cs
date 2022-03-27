namespace CA.Assessment.Domain.Anemic;

public sealed class BlogPost
{
    public BlogPost(Guid identity,
        string title,
        string author,
        string content,
        Guid image,
        IEnumerable<Guid> tags,
        Guid category)
    {
        Identity = identity;
        Title = title ?? throw new ArgumentNullException(nameof(title));
        Author = author ?? throw new ArgumentNullException(nameof(author));
        Content = content ?? throw new ArgumentNullException(nameof(content));
        Image = image;
        Tags = tags ?? throw new ArgumentNullException(nameof(tags));
        Category = category;
    }

    public Guid Identity { get; }

    public string Title { get; }

    public string Author { get; }

    public string Content { get; }

    public Guid Image { get; }

    public IEnumerable<Guid> Tags { get; }

    public Guid Category { get; }
}
