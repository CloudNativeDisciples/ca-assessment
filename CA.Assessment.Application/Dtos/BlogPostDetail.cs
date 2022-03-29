namespace CA.Assessment.Application.Dtos;

public sealed class BlogPostDetail
{
    public BlogPostDetail(
        Guid identity,
        string author,
        string content,
        string title,
        Guid image,
        CategoryDetails category,
        IEnumerable<TagDetails> tags)
    {
        Identity = identity;
        Author = author ?? throw new ArgumentNullException(nameof(author));
        Content = content ?? throw new ArgumentNullException(nameof(content));
        Title = title ?? throw new ArgumentNullException(nameof(title));
        Image = image;
        Category = category ?? throw new ArgumentNullException(nameof(category));
        Tags = tags ?? throw new ArgumentNullException(nameof(tags));
    }

    public Guid Identity { get; }

    public string Author { get; }

    public string Content { get; }

    public string Title { get; }

    public Guid Image { get; }

    public CategoryDetails Category { get; }

    public IEnumerable<TagDetails> Tags { get; }
}
