namespace CA.Assessment.Application.Responses;

public sealed class BlogPostDetails
{
    public Guid Identity { get; }

    public string Author { get; }

    public string Content { get; }

    public string Title { get; }

    public Guid Image { get; }

    public CategoryDetails Category { get; }

    public IEnumerable<TagDetails> Tags { get; }

    public BlogPostDetails(
        Guid identity,
        string author,
        string content,
        string title,
        Guid image,
        CategoryDetails category,
        IEnumerable<TagDetails> tags)
    {
        Identity = identity;
        Image = image;

        Author = author ?? throw new ArgumentNullException(nameof(author));
        Content = content ?? throw new ArgumentNullException(nameof(content));
        Title = title ?? throw new ArgumentNullException(nameof(title));
        Category = category ?? throw new ArgumentNullException(nameof(category));
        Tags = tags ?? throw new ArgumentNullException(nameof(tags));
    }
}
